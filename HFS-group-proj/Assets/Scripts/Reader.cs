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
        //string[] drives = Environment.GetLogicalDrives();
        //UnityDirectory root = new UnityDirectory(RootPath == "" ? drives[0] : RootPath, 0);

        UnityDirectory root = new UnityDirectory("D:/Users/", 0);

        NLT.Node treeRoot = root.convertToTree();

        // Calculate positions for tree nodes, place them
        treeRoot.layout();
        //treeRoot.project(treeRoot);

        Place(treeRoot, null);

        Rig.MoveCameraToWorldLocation(treeRoot.center + new Vector3 (0, 1,0));
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
        node.ob = Instantiate(EntryType[2], node.center + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        node.ob.transform.localScale = new Vector3(node.w - 1, 0.1f, node.h - 1);
        node.ob.name = node.Path;
        node.ob.AddComponent<TeleportationAnchor>();
        // Draw File Ring
        if (node.Directory.Children.Count > 0)
        {

            float arcLength = (360 / node.Directory.Children.Count);
            float distance = node.Directory.Children.Count * 0.2f;
            for (int i = 0; i < node.Directory.Children.Count; i++)
            {
                UnityFileSystemEntry child = node.Directory.Children[i];

                float degrees = i * arcLength + 90;
                float radians = degrees * Mathf.Deg2Rad;
                Vector3 circlePos = new Vector3(Mathf.Cos(radians) * distance, 1, Mathf.Sin(radians) * distance);

                GameObject fileSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fileSphere.transform.position = node.center + circlePos;
                fileSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                Renderer renderer = fileSphere.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", child.EntryType == UnityFileSystemEntry.Type.File ? Color.red : Color.blue);
            }
        }

        // Draw line between current node and parent
        if (parent != null) 
        {
            node.ob.transform.SetParent(parent.ob.transform);

            LineRenderer lineRenderer = node.ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, node.center);
            lineRenderer.SetPosition(1, parent.center);
        }

        foreach (var child in node.c) 
        {
            Place(child, node);
        }
    }
}