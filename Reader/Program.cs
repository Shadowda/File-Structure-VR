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
        public string LastModified { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // obtain names of all logical drives on the computer.
            String[] drives = Environment.GetLogicalDrives();
            Console.WriteLine("GetLogicalDrives: {0}", String.Join(", ", drives));

            //RecursiveFileProcessor
            ProcessDirectory(drives[0], 0);

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

        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory, int depth)
        {
            if (depth > 1)
                return;

            FileOrDirectory d = new FileOrDirectory();
            d.Path = targetDirectory;
            d.name = 


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


            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

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
                ProcessDirectory(subdirectory, depth + 1);

            Console.WriteLine("Processed dir '{0}'.\n", targetDirectory);
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(targetDirectory);
            Console.WriteLine(dirInfo.LastWriteTime);

        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            Console.WriteLine("Processed file '{0}'.", path);
        }
    }
}
