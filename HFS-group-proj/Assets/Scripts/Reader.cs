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

        UnityDirectory root = new UnityDirectory("D:/Users/Pierce T Jackson/Documents/Data Vis/VR/File-Structure-VR/VR TEST FILE STRUCTURE", 0);

        NLT.Node treeRoot = root.convertToTree();

        // Calculate positions for tree nodes, place them and move camera
        treeRoot.layout();
        //treeRoot.project(treeRoot);

        //Rig.MoveCameraToWorldLocation(treeRoot.center + new Vector3 (0, 1,0));
        PlaceTree(treeRoot, null);

        treeRoot.Ring.EnableActions();
        Rig.MoveCameraToWorldLocation(treeRoot.GetCenter());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceTree(NLT.Node node, NLT.Node parent) 
    {
        // Find the center of a node in world space, instantiate a disk at said center
        node.ob = Instantiate(EntryType[2], node.center + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        node.ob.transform.localScale = new Vector3(node.w - 1, 0.1f, node.h - 1);
        node.ob.name = node.Path;
        node.ob.AddComponent<TeleportationAnchor>();

        node.Ring.PlaceFileRing(node.center);
        node.Ring.RingObject.transform.SetParent(node.ob.transform);

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
            PlaceTree(child, node);
        }
    }
}