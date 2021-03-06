using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnityDirectory : UnityFileSystemEntry
{
    public List<UnityFileSystemEntry> Children { get; set; }

    public int Size { get; set; }
    public float width { get; set; }
    public float height { get; set; }
    public float y { get; set; }

    public List<UnityDirectory> GraphedChildren { get; set; }

    public UnityDirectory(string path, int depth = 0, UnityDirectory parent = null) : base(parent)
    {
        System.IO.DirectoryInfo Info = new System.IO.DirectoryInfo(path);

        this.Path = path;
        this.Name = Info.Name;
        this.CreationTime = Info.CreationTime;
        this.LastWriteTime = Info.LastWriteTime;

        this.EntryType = Type.Directory;
        this.Parent = parent;
        this.Position = new Vector3(0f, 0f, 0f);
        this.Children = new List<UnityFileSystemEntry>();
        this.GraphedChildren = new List<UnityDirectory>();

        // Populate directory children if we aren't at max depth
        if (depth < Reader.readDepth)
        {
            ProcessChildren(depth + 1);
        }
        
        this.width = (float)Math.Log(this.Size + 1) * 3;
        this.height = (float)Math.Log(this.Size + 1) * 3;
        this.y = depth;
        
        if (Parent == null)
        {
            foreach (var child in this.GraphedChildren)
            {
                updateY(child);
            }
        }
    }

    public void updateY(UnityDirectory node)
    {
        node.y = node.Parent.y + node.Parent.height + 2;

        if (node.Size / 2 > 20)
        {
            Debug.Log(node.Path);
            Debug.Log(node.Size);
        }

        foreach (var child in node.GraphedChildren)
        {
            updateY(child);
        }
    }

    private void ProcessChildren(int depth) 
    {
        if (depth > Reader.readDepth) { return; }

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
            UnityDirectory dir = new UnityDirectory(directory, depth, this);
            GraphedChildren.Add(dir); // Directories only -> will turn into tree
            Children.Add(dir);
        }

        // Add files to current directory's children
        foreach (string file in files) 
        {
            Children.Add(new UnityFile(file, this));
        }

        Size = Children.Count;
    }

    public void LogPrint(UnityDirectory node)
    {
        foreach (var directory in node.GraphedChildren)
        {
            LogPrint(directory);
        }

        Debug.Log(node.Path);
        Debug.Log(node.width);
        Debug.Log(node.height);
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

    public NLT.Node convertToTree() 
    {
        return convert(this);
    }

    public NLT.Node convert(UnityDirectory node)
    {
        if (node == null) return null;

        List<NLT.Node> children = new List<NLT.Node>();
        foreach (var child in node.GraphedChildren)
        {
            children.Add(convert(child));
        }

        NLT.Node NLTNode = new NLT.Node(node, children);
        foreach (var child in NLTNode.c) 
        {
            child.Parent = NLTNode;
        }

        return NLTNode;
    }
}