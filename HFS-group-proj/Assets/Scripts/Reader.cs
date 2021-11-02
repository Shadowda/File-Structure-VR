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

        // Calculate positions for tree nodes, place them and move camera
        treeRoot.layout();
        PlaceTree(treeRoot, null);

        treeRoot.Ring.EnableActions();
        Rig.MoveCameraToWorldLocation(treeRoot.GetCenter());
    }

    public void PlaceTree(NLT.Node node, NLT.Node parent) 
    {
        // Find the center of a node in world space, instantiate a disk at said center
        Vector3 center = node.GetCenter();
        GameObject ob = Instantiate(EntryType[2], center + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob.transform.localScale = new Vector3(node.w - 1, 0.1f, node.h - 1);
        ob.name = node.Path;

        // Draw line between parent and current node
        if (parent != null) 
        {
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            Vector3 parentCenter = parent.GetCenter();
            lineRenderer.SetPosition(0, center);
            lineRenderer.SetPosition(1, parentCenter);
        }

        foreach (var child in node.c) 
        {
            PlaceTree(child, node);
        }

        node.PlaceFileRing();
    }
}