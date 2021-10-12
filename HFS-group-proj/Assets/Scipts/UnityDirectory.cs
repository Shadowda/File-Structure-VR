using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDirectory : UnityFileSystemEntry
{

    public static int PROCESS_DEPTH_MAX = 1;

    public UnityDirectory Parent { get; set; }

    public System.IO.DirectoryInfo Info { get; set; }
    public List<UnityFileSystemEntry> Children { get; set; }

    public int Size { get; set; }

    // position in space
    public float Mod { get; set; }
    public float Width { get; set; }
    public int Height { get; set; }
    public float X { get; set; }
    public int Y { get; set; }
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
        // Populate directory children if we aren't at max depth
        if (depth < PROCESS_DEPTH_MAX)
        {
            ProcessChildren(depth + 1);
        }
    }

    private void ProcessChildren(int depth)
    {
        if (depth > 1) { return; }

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
            Children.Add(new UnityDirectory(directory, depth, this));
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
                this.GraphedChildren.Add(Children[i]);
            }
        }
    }

    // graphing

    public bool IsLeaf()
    {
        return this.GraphedChildren.Count == 0;
    }

    public bool IsLeftMost()
    {
        if (this.Parent == null)
            return true;

        return this.Parent.GraphedChildren[0] == this;
    }

    public bool IsRightMost()
    {
        if (this.Parent == null)
            return true;

        return this.Parent.GraphedChildren[this.Parent.GraphedChildren.Count - 1] == this;
    }

    public UnityDirectory GetPreviousSibling()
    {
        if (this.Parent == null || this.IsLeftMost())
            return null;

        return this.Parent.GraphedChildren[this.Parent.GraphedChildren.IndexOf(this) - 1];
    }

    public UnityDirectory GetNextSibling()
    {
        if (this.Parent == null || this.IsRightMost())
            return null;

        return this.Parent.GraphedChildren[this.Parent.GraphedChildren.IndexOf(this) + 1];
    }

    public UnityDirectory GetLeftMostSibling()
    {
        if (this.Parent == null)
            return null;

        if (this.IsLeftMost())
            return this;

        return this.Parent.GraphedChildren[0];
    }

    public UnityDirectory GetLeftMostChild()
    {
        if (this.GraphedChildren.Count == 0)
            return null;

        return this.GraphedChildren[0];
    }

    public UnityDirectory GetRightMostChild()
    {
        if (this.GraphedChildren.Count == 0)
            return null;

        return this.GraphedChildren[GraphedChildren.Count - 1];
    }

    public void LogPrint(UnityDirectory node)
    {
        Debug.Log(node.Name);
        foreach (var child in node.Children)
        {
            var child2 = child as UnityDirectory;
            if (child2 != null)
            {
                LogPrint(child2);
            }
        }
    }

}