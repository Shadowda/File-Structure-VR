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

    public void PlaceSphere(UnityFileSystemEntry entry, NLT.Node node, Vector3 position, bool addToFileObjectList = true) 
    {
        // Create a sphere, optionally attach the sphere to the rotating ring
        GameObject fileSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (addToFileObjectList) 
        {
            fileSphere.transform.parent = RingObject.transform;
            fileSphere.transform.localPosition = position;
            FileObjects.Add(fileSphere);
        }
        else 
        {
            fileSphere.transform.position = position;
            BackObject = fileSphere;
            BackObject.SetActive(false);
        }
        fileSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Create new object to hold text
        GameObject txtHolder = new GameObject();

        // Make fileSphere parent of txtholder
        txtHolder.transform.parent = fileSphere.transform;
        txtHolder.name = "Invalid";

        // Create text mesh
        TextMesh textMesh = txtHolder.AddComponent<TextMesh>();
        textMesh.text = entry.Name;
        textMesh.characterSize = (float)0.01;

        // Set postion of the TextMesh with offset
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.transform.position = new Vector3(
            fileSphere.transform.position.x,
            fileSphere.transform.position.y,
            fileSphere.transform.position.z
        );

        // Give each sphere the proper color
        Renderer renderer = fileSphere.GetComponent<Renderer>();
        if (entry.EntryType == UnityFileSystemEntry.Type.File) 
        {
            renderer.material.SetColor("_Color", Color.red);
        }
        else 
        {
            renderer.material.SetColor("_Color", Color.blue);
            DirectoryInteractable dirInteractable = fileSphere.AddComponent<DirectoryInteractable>();
            dirInteractable.Position = node.GetCenter();
            dirInteractable.RingToEnable = node.Ring;
            dirInteractable.RingToDisable = this;
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
            PlaceSphere(Node.Parent.Directory, Node.Parent, backPos, false);
        }

        // Iterate through UnityDirectory children to render directory and file spheres
        for (int i = 0; i < Directory.Children.Count; i++) 
        {
            UnityFileSystemEntry child = Directory.Children[i];
            NLT.Node childNode = Node.c.Find(node => node.Path == child.Path);

            float degrees = (i * arcLength) + centerOffset;
            float radians = degrees * Mathf.Deg2Rad;
            Vector3 circlePos = new Vector3(Mathf.Cos(radians) * distance, 1, Mathf.Sin(radians) * distance);

            PlaceSphere(child, childNode, circlePos);
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
