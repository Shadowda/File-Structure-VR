using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reader : MonoBehaviour
{
    public String RootPath;
    public GameObject[] EntryType;
    public XRRig Rig;

    // Start is called before the first frame update
    void Start()
    {
        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(RootPath, 0);

        DrawTreeTest.TreeHelpers.CalculateNodePositions(root);
        Place(root);

        Rig.MoveCameraToWorldLocation(new Vector3(root.X, 0, root.Y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Place(UnityDirectory node)
    {
        Vector3 spawnPosition = new Vector3(node.X, 0f, node.Y * 15);
        var ob = Instantiate(EntryType[(int)node.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob.name = node.Path;

        PlaceFiles(node);

        if (node.Parent != null) 
        { 
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            Vector3 spawnPosition2 = new Vector3(node.Parent.X, 0f, node.Parent.Y * 15);
            lineRenderer.SetPosition(0, spawnPosition);
            lineRenderer.SetPosition(1, spawnPosition2);

        }

        foreach (var child in node.Children)
        {
            var child2 = child as UnityDirectory;
            if (child2 != null)
            {
                Place(child2);
            }
        }
    }

    public void PlaceFiles(UnityDirectory node) 
    {
        for (int i = 0; i < node.Children.Count; i++)
        {
            UnityFileSystemEntry fsEntry = node.Children[i];
            GameObject fsObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fsObject.transform.position = new Vector3(node.X, i + 1, node.Y * 15);
            fsObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Renderer renderer = fsObject.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", fsEntry.EntryType == UnityFileSystemEntry.Type.File ? Color.red : Color.blue);
        }
    }
}