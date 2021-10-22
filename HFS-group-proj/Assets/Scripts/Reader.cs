using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reader : MonoBehaviour
{
    public string RootPath;
    public XRRig Rig;
    public GameObject[] EntryType;

    // Main - called before the first frame update
    void Start()
    {
        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(RootPath == "" ? drives[0] : RootPath, 0);
        NLT.Node treeRoot = root.convertToTree();

        // Calculate positions for tree nodes, place them
        treeRoot.layout();
        Place(treeRoot, null);

        Rig.MoveCameraToWorldLocation(GetNodeCenter(treeRoot));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 GetNodeCenter(NLT.Node node) 
    {
        return new Vector3((node.x * 2 + node.w - 1) * 0.5f, 0, (node.y * 2 + node.h) * 0.5f);
    }

    public void Place(NLT.Node node, NLT.Node parent) 
    {
        // Find the center of a node in world space, instantiate a disk at said center
        Vector3 center = GetNodeCenter(node);
        GameObject ob = Instantiate(EntryType[2], center + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob.transform.localScale = new Vector3(node.w - 1, 0.1f, node.h - 1);
        ob.name = node.Path;

        // Draw File Ring
        float arcLength = (360 / node.Directory.Children.Count);
        float distance = node.Directory.Children.Count * 0.2f;
        for (int i = 0; i < node.Directory.Children.Count; i++)
        {
            UnityFileSystemEntry child = node.Directory.Children[i];

            float degrees = i * arcLength + 90;
            float radians = degrees * Mathf.Deg2Rad;
            Vector3 circlePos = new Vector3(Mathf.Cos(radians) * distance, 1, Mathf.Sin(radians) * distance);

            GameObject fileSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fileSphere.transform.position = center + circlePos;
            fileSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            Renderer renderer = fileSphere.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", child.EntryType == UnityFileSystemEntry.Type.File ? Color.red : Color.blue);
        }

        // Draw line between parent and current node
        if (parent != null) 
        {
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            Vector3 parentCenter = GetNodeCenter(parent);
            lineRenderer.SetPosition(0, center);
            lineRenderer.SetPosition(1, parentCenter);
        }

        foreach (var child in node.c) 
        {
            Place(child, node);
        }
    }
}