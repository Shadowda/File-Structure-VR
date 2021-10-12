using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityFile : UnityFileSystemEntry
{

    public System.IO.FileInfo Info { get; set; }
    public UnityDirectory Parent { get; set; }

    public UnityFile(string path, UnityDirectory parent) : base(parent)
    {
        Parent = parent;
        Info = new System.IO.FileInfo(path);
        EntryType = UnityFileSystemEntry.Type.File;
        Path = path;
        Name = Info.Name;
        Length = Info.Length;
        LastModified = Info.LastWriteTime;
        Position = new Vector3(0f, 0f, 0f);
    }
}