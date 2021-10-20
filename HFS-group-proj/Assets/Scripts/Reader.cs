using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NLT;

public class Reader : MonoBehaviour
{
    public GameObject[] EntryType;

    // Main - called before the first frame update
    void Start()
    {

        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();

        //UnityDirectory root = new UnityDirectory(drives[0], 0);

        UnityDirectory root = new UnityDirectory("D:/Users/", 0);

        NLT_Tree.Tree treeRoot = root.convert(root);

        treeRoot.layout(treeRoot);
        //root.LogPrint(root);

        Place(treeRoot, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Place(NLT_Tree.Tree node, NLT_Tree.Tree Parent)
    {
        //Vector3 spawnPosition = new Vector3(node.x * 2, 0, node.y * 20);
        //GameObject ob = Instantiate(EntryType[2], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

        Vector3 s1 = new Vector3(node.x, 0, node.y);
        Vector3 s2 = new Vector3(node.x + node.w - 1, 0, node.y);
        Vector3 s3 = new Vector3(node.x, 0, node.y + node.h);
        Vector3 s4 = new Vector3(node.x + node.w - 1, 0, node.y + node.h);

        float xc = (node.x + node.x + node.w - 1) / 2;
        float yc = (node.y + node.y + node.h) / 2;

        Vector3 cen = new Vector3(xc, 0, yc);

        GameObject ob = Instantiate(EntryType[2], cen + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob.name = node.Path;
        ob.transform.localScale = new Vector3(node.w - 1, 1, node.h - 1);

        GameObject ob1 = Instantiate(EntryType[1], s2 + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        GameObject ob2 = Instantiate(EntryType[1], s3 + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        GameObject ob3 = Instantiate(EntryType[1], s4 + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob1.transform.SetParent(ob.transform);
        ob2.transform.SetParent(ob.transform);
        ob3.transform.SetParent(ob.transform);

        if (Parent != null)
        {
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            xc = (Parent.x + Parent.x + Parent.w - 1) / 2;
            yc = (Parent.y + Parent.y + Parent.h) / 2;
            Vector3 cen2 = new Vector3(xc, 0, yc);
            //Vector3 spawnPosition2 = new Vector3(Parent.x, 0, Parent.y);

            lineRenderer.SetPosition(0, cen);
            lineRenderer.SetPosition(1, cen2);
        }

        foreach (var child in node.c)
        {
            Place(child, node);
        }
    }

}