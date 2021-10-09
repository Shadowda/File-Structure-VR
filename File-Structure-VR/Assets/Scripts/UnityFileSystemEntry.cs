using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityFileSystemEntry {

    public enum Type { Directory, File }

    public UnityFileSystemEntry Parent { get; set; }
    public Type EntryType { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public long Length { get; set; }
    public DateTime LastModified { get; set; }

    public UnityFileSystemEntry(UnityFileSystemEntry parent) {
        Parent = parent;
    }
}