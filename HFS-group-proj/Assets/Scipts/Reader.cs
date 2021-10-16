using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reader : MonoBehaviour
{
    public GameObject[] EntryType;

    // Start is called before the first frame update
    void Start()
    {

        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        
        //UnityDirectory root = new UnityDirectory(drives[0], 0);

        RootMaster master = new RootMaster();
        master.Name = "test";

        UnityDirectory root = new UnityDirectory("D:/Users/", 0);
        root.Master = master;
        root.updateMaster(root);

        //root.LogPrint(root);

        DrawTreeTest.TreeHelpers.CalculateNodePositions(root);
        Place(root);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Place(UnityDirectory node)
    {
        Vector3 spawnPosition = new Vector3(node.X, 0 , node.Y * 15);

        //Vector3 spawnPosition = new Vector3(node.X, -node.Y * 15, node.Z);

        node.ob = Instantiate(EntryType[2], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        node.ob.name = node.Path;

        //Debug.Log(node.ob.name);
        //Debug.Log(node.ob.transform.position);
        //Debug.Log(node.ob.transform.localPosition);

        if (node.Parent != null) { 
            node.ob.transform.SetParent(node.Parent.ob.transform);
            //node.ob.transform.localPosition += new Vector3(0, 10, 0);
            //node.ob.transform.SetParent(null);
        }

        //ob.transform.localScale = ob.transform.localScale + new Vector3(node.Size/10, 0, node.Size/10);

        //var ob = Instantiate(EntryType[(int)node.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

        
        if (node.Parent != null) { 
            LineRenderer lineRenderer = node.ob.AddComponent<LineRenderer>();
            //lineRenderer.useWorldSpace = false;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            //lineRenderer.SetWidth(0, node.Size/10);
            lineRenderer.positionCount = 2;

            Vector3 spawnPosition2 = new Vector3(node.Parent.X, 0, node.Parent.Y * 15);
            //Vector3 spawnPosition2 = new Vector3(node.Parent.X, -node.Parent.Y * 15, node.Parent.Z);
        
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
}