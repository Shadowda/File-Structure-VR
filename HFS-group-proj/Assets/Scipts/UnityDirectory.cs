using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDirectory : UnityFileSystemEntry
{

    public static int PROCESS_DEPTH_MAX = 6;

    public UnityDirectory Parent { get; set; }

    public System.IO.DirectoryInfo Info { get; set; }
    public List<UnityFileSystemEntry> Children { get; set; }

    public int Size { get; set; }

    public GameObject ob { get; set; }

    public float width { get; set; }
    public float height { get; set; }

    public float x { get; set; }
    public float y { get; set; }

    public List<UnityDirectory> GraphedChildren { get; set; }

    public UnityDirectory(string path, int depth = 0, UnityDirectory parent = null) : base(parent)
    {
        Parent = parent;
        Info = new System.IO.DirectoryInfo(path);
        EntryType = UnityFileSystemEntry.Type.Directory;
        Path = path;
        Name = Info.Name;
        LastModified = Info.LastWriteTime;
        Position = new Vector3(0f, 0f, 0f);
        Children = new List<UnityFileSystemEntry>();
        GraphedChildren = new List<UnityDirectory>();

        // Populate directory children if we aren't at max depth
        if (depth < PROCESS_DEPTH_MAX)
        {
            ProcessChildren(depth + 1);
        }

        width = 2;
        height = 2;
        y = (float) depth;

    }

    private void ProcessChildren(int depth)
    {
        if (depth > PROCESS_DEPTH_MAX) { return; }

        string[] directories, files;

        // Attempt to read in the files and directories at a path
        try
        {
            directories = System.IO.Directory.GetDirectories(Path);
            files = System.IO.Directory.GetFiles(Path);
        }
        catch (System.UnauthorizedAccessException)
        {
            directories = new string[0];
            files = new string[0];
        }

        // Add directories to current directory's children
        foreach (string directory in directories)
        {
            if (directory != "D:/Users/Pierce T Jackson/Documents") { 
                Children.Add(new UnityDirectory(directory, depth, this));
            }
        }

        // Add files to current directory's children
        foreach (string file in files)
        {
            Children.Add(new UnityFile(file, this));
        }

        Size = Children.Count;
        
        for (int i = 0; i < Size; i++)
        {
            var child = Children[i] as UnityDirectory;
            if (child != null)
            {
                this.GraphedChildren.Add(child);
            }
        }
    }






    public void LogPrint(UnityDirectory node)
    {
        foreach (var child in node.Children)
        {
            var child2 = child as UnityDirectory;
            if (child2 != null)
            {
                LogPrint(child2);
            }
        }
        
        Debug.Log(node.Path);
        Debug.Log(node.Master.Name);
      
       // Debug.Log(node.X);
        //Debug.Log(node.Y);
        //Debug.Log(' ');
    }

    public void Log<t>(t str)
    {
        Debug.Log(str);
    }

    public void updateMaster(UnityDirectory node)
    {
        foreach (var child in node.GraphedChildren)
        {
            child.Master = Master;
            updateMaster(child);
        }
    }

    public NLT.NLT_Tree.Tree convert(UnityDirectory root)
    {
        if (root == null) return null;

        List<NLT.NLT_Tree.Tree> children = new List<NLT.NLT_Tree.Tree>();

        foreach (var child in root.GraphedChildren)
        {
            children.Add(convert(child));
        }

        return new NLT.NLT_Tree.Tree(root.width, root.height, root.y, children);
    }


}