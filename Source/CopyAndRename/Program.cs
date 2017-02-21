using System;
using System.IO;

namespace CopyAndRename
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pathToPictures = @"D:\Pixum\Pixum2015\_bruges\_bruges\_bruges";
            var destName = @"d:\temp\pixum" + DateTime.Now.ToString("HHmmss");

            Directory.CreateDirectory(destName);

            var i = 0;

            foreach (var file in Directory.EnumerateFiles(pathToPictures, "*.*"))
            {
                var ext = Path.GetExtension(file).ToLower();
                var newName = i.ToString("000") + ext;

                File.Copy(file,Path.Combine( destName, newName));
                i++;
            }
        }
    }
}