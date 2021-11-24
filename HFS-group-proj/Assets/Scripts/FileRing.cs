using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class FileRing
{
    public List<GameObject> FileObjects;
    public GameObject BackObject;
    public GameObject RingObject;
    public InputActionAsset ActionsAsset;

    private UnityDirectory Directory;
    private NLT.Node Node;
    private float Radius;

    public FileRing(ref UnityDirectory directory, NLT.Node node)
    {
        Directory = directory;
        Node = node;
        Radius = directory.width;

        // Dynamically add rotate action script to Ring GameObject
        RingObject = new GameObject();
        RingObject.name = "Ring \"" + Directory.Name + "\"";
        ActionsAsset = new XRIDefaultInputActions().asset;
        InputAction inputAction = ActionsAsset.FindAction("HFS CustomActions/Rotate");
        RotateAction rotateScript = RingObject.AddComponent<RotateAction>();
        rotateScript.rotateReference = InputActionReference.Create(inputAction);

        FileObjects = new List<GameObject>();
    }

    public void PlaceFSEntryObject(UnityFileSystemEntry entry, NLT.Node node, Vector3 position, bool addToFileObjectList = true) 
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
        textMesh.characterSize = (float)0.01;

        // Set postion of the TextMesh with offset
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.transform.position = new Vector3(
            fsEntryObject.transform.position.x,
            fsEntryObject.transform.position.y,
            fsEntryObject.transform.position.z
        );

        if (addToFileObjectList) 
        {
            fsEntryObject.transform.parent = RingObject.transform;
            fsEntryObject.transform.localPosition = position;
            FileObjects.Add(fsEntryObject);
        }
        else 
        {
            fsEntryObject.transform.position = position;
            BackObject = fsEntryObject;
            BackObject.SetActive(false);
        }
    }

    public void Place(Vector3 position)
    {
        RingObject.transform.position = position;

        // Constants for sphere ring sphere placement about a circle
        float arcLength = (360 / (Directory.Children.Count + 20));
        float centerOffset = 90 - (arcLength * (Directory.Children.Count - 1) * 0.5f);
        float distance = Radius * 0.333f;

        // Place a sphere for the "back" button
        if (Node.Parent != null) 
        {
            Vector3 backPos = Node.GetCenter();
            backPos.y += 0.5f;
            backPos.z -= distance;
            PlaceFSEntryObject(Node.Parent.Directory, Node.Parent, backPos, false);
        }

        // Iterate through UnityDirectory children to render directory and file spheres
        for (int i = 0; i < Directory.Children.Count; i++) 
        {
            UnityFileSystemEntry child = Directory.Children[i];
            NLT.Node childNode = Node.c.Find(node => node.Path == child.Path);

            float degrees = (i * arcLength) + centerOffset;
            float radians = degrees * Mathf.Deg2Rad;
            Vector3 circlePos = new Vector3(Mathf.Cos(radians) * distance, 1, Mathf.Sin(radians) * distance);

            PlaceFSEntryObject(child, childNode, circlePos);
        }
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
