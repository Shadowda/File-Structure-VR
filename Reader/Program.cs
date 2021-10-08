using System;
using System.Collections.Generic;

namespace Reader
{
    class FileOrDirectory
    {
        public List<FileOrDirectory> Children { get; set; }
        public FileOrDirectory Parent { get; set; }public string Path { get; set; }
        
        public string Name { get; set; }
        
        // size (number_of_items) dervied: Children.Count
        public int Size { get; set; }
        public DateTime LastModified { get; set; }

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public FileOrDirectory ProcessDirectory(string targetDirectory, int depth)
        {
            if (depth > 1)
                return null;

            // Process the list of files found in the directory.
            string[] fileEntries;

            try
            {
                fileEntries = System.IO.Directory.GetFiles(targetDirectory);
            }
            catch (System.UnauthorizedAccessException)
            {
                fileEntries = new string[0];
            }

            var childTree = new List<FileOrDirectory>();

            foreach (string fileName in fileEntries)
                childTree.Add(ProcessFile(fileName));

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries;

            try
            {
                subdirectoryEntries = System.IO.Directory.GetDirectories(targetDirectory);
            }
            catch (System.UnauthorizedAccessException)
            {
                subdirectoryEntries = new string[0];
            }

            foreach (string subdirectory in subdirectoryEntries)
                childTree.Add(ProcessDirectory(subdirectory, depth + 1));

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(targetDirectory);
            FileOrDirectory d = new FileOrDirectory();
            d.Path = targetDirectory;
            d.Name = dirInfo.Name;
            d.Size = childTree.Count;
            d.LastModified = dirInfo.LastWriteTime;
            d.Children = childTree;


            Console.WriteLine("Processed dir '{0}'.\n", targetDirectory);
            //System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(targetDirectory);
            //Console.WriteLine(dirInfo.LastWriteTime);
            return d;
        }

        // Insert logic for processing found files here.
        public FileOrDirectory ProcessFile(string path)
        {
            var fInfo = new System.IO.FileInfo(path);

            FileOrDirectory f = new FileOrDirectory();
            f.Path = path;
            f.Name = fInfo.Name;
            f.Size = 0;
            f.LastModified = fInfo.LastWriteTime;
            f.Children = null;

            return f;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // obtain names of all logical drives on the computer.
            String[] drives = Environment.GetLogicalDrives();
            Console.WriteLine("GetLogicalDrives: {0}", String.Join(", ", drives));

            //RecursiveFileProcessor
            FileOrDirectory root = new FileOrDirectory();
            root = root.ProcessDirectory(drives[0], 0);

            Console.WriteLine(root.Path);
            Console.WriteLine(root.Name);
            Console.WriteLine(root.Size);
            Console.WriteLine(root.Children[0].Path);
            Console.WriteLine(root.Children[0].Name);

            { 
            /*
            // drive info for first drive on computer
            System.IO.DriveInfo di = new System.IO.DriveInfo(@drives[0]);
            System.IO.DirectoryInfo dirInfo = di.RootDirectory;
            Console.WriteLine(dirInfo.Attributes.ToString());

            // Get the files in the directory and print out some information about them.
            System.IO.FileInfo[] fileNames = dirInfo.GetFiles("*.*");

            foreach (System.IO.FileInfo fi in fileNames)
            {
                Console.WriteLine("{0}: {1}: {2}", fi.Name, fi.LastAccessTime, fi.Length);
            }

            System.IO.DirectoryInfo[] dirInfos = dirInfo.GetDirectories("*.*");

            // Get the subdirectories directly that is under the root.
            // See "How to: Iterate Through a Directory Tree" for an example of how to
            // iterate through an entire tree.
            foreach (System.IO.DirectoryInfo d in dirInfos)
            {
                Console.WriteLine(d.Name);
            }
            */}
        }
    }
}
