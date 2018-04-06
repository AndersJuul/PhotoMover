using System;
using System.Collections.Generic;
using System.IO;

namespace AJF.PhotoMover.CopyToFlat
{
    internal class Program
    {
        const string SourceDir = @"D:\Raw\";
        private static void Main(string[] args)
        {

            var fileList = new List<string>();

            Populate(SourceDir, fileList);

            Console.WriteLine("File count: " + fileList.Count);

            fileList.Sort();

            var fList = new List<FileClass>();
            foreach (var file in fileList)
            {
                fList.Add(new FileClass(file,SourceDir));
            }

            foreach (var fileClass in fList)
            {
                var from = fileClass.FilePathFrom;
                var to = fileClass.FilePathTo;
                Console.WriteLine(from + "     " + to);
                File.Copy(from, to,false);
            }
        }

        private static void Populate(string dir, List<string> fileList)
        {
            Console.WriteLine("Looking through " + dir);

            var enumerateFiles = Directory.EnumerateFiles(dir);
            var enumerateDirectories = Directory.EnumerateDirectories(dir);

            fileList.AddRange(enumerateFiles);

            foreach (var directory in enumerateDirectories)
            {
                Populate(directory, fileList);
            }
        }
    }

    internal class FileClass
    {
       public string FilePathFrom { get; }
        public string RootDir { get; }

        public string FilePathTo
        {
            get
            {
                var root = RootDir.Length;
                var rootPart = FilePathFrom.Substring(0,root);
                var subDirPart = FilePathFrom.Substring(root).Replace("\\","-").Replace(" ","-");
                return rootPart+subDirPart;
            }
        }

        public FileClass(string filePathFrom, string rootDir)
        {
            FilePathFrom = filePathFrom;
            RootDir = rootDir;
        }
    }
}