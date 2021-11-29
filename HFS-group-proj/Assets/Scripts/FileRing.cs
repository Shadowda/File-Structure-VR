using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public enum SortBy
{
    ReadOrder,
    Name,
    CreationTime,
    LastModifiedTime
}

public class FileRing
{
    public static SortBy[] SortByArray = { SortBy.ReadOrder, SortBy.Name, SortBy.CreationTime, SortBy.LastModifiedTime };

    public GameObject BackObject;
    public GameObject RingObject;
    public InputActionAsset ActionsAsset;

    private UnityDirectory Directory;
    private NLT.Node Node;
    private float Radius;
    private int SortIndex;

    private float ArcLength;
    private float CenterOffset;
    private float Distance;

    public FileRing(ref UnityDirectory directory, NLT.Node node)
    {
        Directory = directory;
        Node = node;
        Radius = directory.width;
        SortIndex = 0;

        RingObject = new GameObject();
        RingObject.name = "Ring \"" + Directory.Name + "\"";

        ActionsAsset = new XRIDefaultInputActions().asset;

        // Dynamically add rotate action script to Ring GameObject
        InputAction inputAction = ActionsAsset.FindAction("HFS CustomActions/Rotate");
        RotateAction rotateScript = RingObject.AddComponent<RotateAction>();
        rotateScript.rotateReference = InputActionReference.Create(inputAction);

        // Dynamically add sort action script to Ring GameObject. Super extra, but okay for now.
        InputAction sortNextAction = ActionsAsset.FindAction("HFS CustomActions/SortNext");
        InputAction sortLastAction = ActionsAsset.FindAction("HFS CustomActions/SortLast");
        SortAction sortActionScript = RingObject.AddComponent<SortAction>();
        sortActionScript.sortNextReference = InputActionReference.Create(sortNextAction);
        sortActionScript.sortLastReference = InputActionReference.Create(sortLastAction);
        sortActionScript.OnInputActionReferenceAssigned();
        sortActionScript.fileRing = this;
    }

    public void PlaceFSEntryObject(UnityFileSystemEntry entry, NLT.Node node, Vector3 position, bool fileRingObject = true) 
    {
        // Divine the correct resource type
        GameObject fsEntryObject;
        string resourcePath = "File";

        if (entry.EntryType == UnityFileSystemEntry.Type.Directory) 
        {
            resourcePath = ((UnityDirectory)entry).Children.Count > 0
                ? "Folder1" 
                : "Folder2";
        }

        GameObject resource = Resources.Load(resourcePath) as GameObject;
        fsEntryObject = GameObject.Instantiate(resource);

        // Give directory prefabs interactability
        if (entry.EntryType == UnityFileSystemEntry.Type.Directory) 
        {
            BoxCollider fsBoxCollider = fsEntryObject.AddComponent<BoxCollider>();
            fsBoxCollider.size = new Vector3(0.35f, 0.35f, 0.35f);

            DirectoryInteractable dirInteractable = fsEntryObject.AddComponent<DirectoryInteractable>();
            dirInteractable.TargetPosition = node.GetCenter();
            dirInteractable.TargetPlatform = GameObject.Find(entry.Path);
            dirInteractable.RingToEnable = node.Ring;
            dirInteractable.RingToDisable = this;
        }

        // Create new object to hold text
        GameObject txtHolder = new GameObject();

        // Make fsEntryObject parent of txtholder
        txtHolder.transform.parent = fsEntryObject.transform;
        txtHolder.name = "Invalid";

        // Create text mesh
        TextMesh textMesh = txtHolder.AddComponent<TextMesh>();
        textMesh.text = entry.Name;
        textMesh.characterSize = (float)0.05;

        // Set postion of the TextMesh with offset
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.transform.position = new Vector3(
            fsEntryObject.transform.position.x,
            fsEntryObject.transform.position.y,
            fsEntryObject.transform.position.z
        );

        if (fileRingObject) 
        {
            fsEntryObject.transform.parent = RingObject.transform;
            fsEntryObject.transform.localPosition = position;
        }
        else 
        {
            fsEntryObject.transform.position = position;
            BackObject = fsEntryObject;
            BackObject.SetActive(false);
        }
    }

    private void PlaceFSEntryObjects()
    {
        // Sort file system entries based on the selected sorting type
        List<UnityFileSystemEntry> fsEntries = new List<UnityFileSystemEntry>(Directory.Children);
        fsEntries.Sort(delegate (UnityFileSystemEntry x, UnityFileSystemEntry y) {
            switch (SortByArray[SortIndex]) {
                case SortBy.Name:
                    return x.Name.CompareTo(y.Name);
                case SortBy.CreationTime:
                    return x.CreationTime.CompareTo(y.CreationTime);
                case SortBy.LastModifiedTime:
                    return x.LastWriteTime.CompareTo(y.LastWriteTime);
                default:
                    return 0;
            }
        });

        // Iterate through UnityDirectory children to render directory and file spheres
        for (int i = 0; i < fsEntries.Count; i++) {
            UnityFileSystemEntry child = fsEntries[i];
            NLT.Node childNode = Node.c.Find(node => node.Path == child.Path);

            float degrees = -(i * ArcLength) + CenterOffset;
            float radians = degrees * Mathf.Deg2Rad;
            Vector3 circlePos = new Vector3(Mathf.Cos(radians) * Distance, 1, Mathf.Sin(radians) * Distance);

            PlaceFSEntryObject(child, childNode, circlePos);
        }
    }

    public void Place(Vector3 position)
    {
        RingObject.transform.position = position;

        // Constants for sphere ring sphere placement about a circle
        ArcLength = (360 / (Directory.Children.Count + 20));
        CenterOffset = (ArcLength * (Directory.Children.Count - 1) * 0.5f) + 90;
        Distance = Radius * 0.333f;

        // Place a sphere for the "back" button
        if (Node.Parent != null) 
        {
            Vector3 backPos = Node.GetCenter();
            backPos.y += 0.5f;
            backPos.z -= Distance;
            PlaceFSEntryObject(Node.Parent.Directory, Node.Parent, backPos, false);
        }

        PlaceFSEntryObjects();
    }

    // Select the next or previous filering sorting method
    public void SelectNextSort(float direction)
    {
        SortIndex = (int)Mathf.Repeat(SortIndex + direction, SortByArray.Length);
        foreach (Transform child in RingObject.transform) 
        {
            GameObject.Destroy(child.gameObject);
        }
        PlaceFSEntryObjects();
    }

    // Enable the ability to spin this ring with the thumbstick
    public void EnableActions()
    {
        BackObject?.SetActive(true);
        ActionsAsset.Enable();
    }

    // Disable the ability to spin this ring with the thumbstick
    public void DisableActions() 
    {
        BackObject?.SetActive(false);
        ActionsAsset.Disable();
    }
}