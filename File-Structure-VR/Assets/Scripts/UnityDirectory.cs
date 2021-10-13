using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDirectory : UnityFileSystemEntry {

    public static int PROCESS_DEPTH_MAX = 1;

    public System.IO.DirectoryInfo Info { get; set; }
    public List<UnityFileSystemEntry> Children { get; set; }

    public UnityDirectory(string path, int depth = 0, UnityFileSystemEntry parent = null) : base(parent) {
        Info = new System.IO.DirectoryInfo(path);
        EntryType = UnityFileSystemEntry.Type.Directory;
        Path = path;
        Name = Info.Name;
        LastModified = Info.LastWriteTime;
        Children = new List<UnityFileSystemEntry>();

        // Populate directory children if we aren't at max depth
        if (depth < PROCESS_DEPTH_MAX) {
            ProcessChildren(depth + 1);
        }
    }

    private void ProcessChildren(int depth) {
        string[] directories, files;

        // Attempt to read in the files and directories at a path
        try {
            directories = System.IO.Directory.GetDirectories(Path);
            files = System.IO.Directory.GetFiles(Path);
        } catch (System.UnauthorizedAccessException) {
            directories = new string[0];
            files = new string[0];
        }

        // Add directories to current directory's children
        foreach (string directory in directories) {
            Children.Add(new UnityDirectory(directory, depth, this));
        }

        // Add files to current directory's children
        foreach (string file in files) {
            Children.Add(new UnityFile(file, this));
        }
    }
}