using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Reader : MonoBehaviour
{
    public string RootPath;
    public int ReadDepth = 6;
    public static int readDepth;
    public XRRig Rig;
    public Boolean PresentationMode = false;
    public GameObject[] EntryType;

    // Main - called before the first frame update
    void Start()
    {
        Reader.readDepth = ReadDepth;

        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(RootPath == "" ? drives[0] : RootPath, 0);
        NLT.Node treeRoot = root.convertToTree();

        // Calculate positions for tree nodes, place them and move camera
        treeRoot.layout();
        PlaceTree(treeRoot, null);

        treeRoot.Ring.EnableActions();
        Rig.MoveCameraToWorldLocation(PresentationMode 
            ? new Vector3(-5, 5, 0)
            : treeRoot.GetCenter()
        );
    }

    public void PlaceTree(NLT.Node node, NLT.Node parent) 
    {
        // Find the center of a node in world space, create constant
        Vector3 center = node.GetCenter();
        Vector3 constellationOffset = new Vector3(0, 25, 0);

        // Create primary platform where interactables will be rendered
        GameObject ob = Instantiate(EntryType[2], center + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob.transform.localScale = new Vector3(node.w - 1, 0.1f, node.h - 1);
        ob.name = node.Path;

        // Create second platform for the constellation
        GameObject ob2 = Instantiate(EntryType[2], center + transform.TransformPoint(constellationOffset), gameObject.transform.rotation);
        ob2.transform.localScale = new Vector3(node.w - 1, 0.1f, node.h - 1);


        // Draw line between parent and current node
        if (parent != null) 
        {
            Vector3 parentCenter = parent.GetCenter();

            // Draw parentage lines between primary platforms
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, center);
            lineRenderer.SetPosition(1, parentCenter);

            // Draw parentage lines between constellation platforms
            LineRenderer lineRenderer2 = ob2.AddComponent<LineRenderer>();
            lineRenderer2.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer2.widthMultiplier = 0.2f;
            lineRenderer2.positionCount = 2;

            lineRenderer2.SetPosition(0, center + constellationOffset);
            lineRenderer2.SetPosition(1, parentCenter + constellationOffset);
        }

        foreach (var child in node.c) 
        {
            PlaceTree(child, node);
        }

        node.PlaceFileRing();
    }
}