using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityFile : UnityFileSystemEntry
{
    public UnityFile(string path, UnityDirectory parent) : base(parent)
    {
        System.IO.FileInfo Info = new System.IO.FileInfo(path);

        Path = path;
        Name = Info.Name;
        Length = Info.Length;
        CreationTime = Info.CreationTime;
        LastWriteTime = Info.LastWriteTime;

        EntryType = Type.File;
        Parent = parent;
        Position = new Vector3(0f, 0f, 0f);
    }
}