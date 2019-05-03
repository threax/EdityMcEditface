using EdityMcEditface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdityMcEditface.Mvc.Tests
{
    /// <summary>
    /// Create a directory that will be deleted when this class is disposed. If the directory already
    /// exists it will be deleted and recreated. Be careful with this.
    /// </summary>
    public class SelfDeletingDirectory : IDisposable
    {
        private String path;

        public SelfDeletingDirectory(String path)
        {
            this.path = path;

            if (Directory.Exists(path))
            {
                DeleteDirectory(path);
            }

            Directory.CreateDirectory(path);
        }

        public void Dispose()
        {
            DeleteDirectory(path);
        }

        public String Path { get => path; }

        private static void DeleteDirectory(string directoryPath)
        {
            //Thanks to SimonCropp at https://github.com/libgit2/libgit2sharp/issues/769
            //Removed setting the directory path back to normal
            //Will do a complete delete, even of readonly files, good for cleaning up git repos.

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            var files = Directory.GetFiles(directoryPath);
            var directories = Directory.GetDirectories(directoryPath);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in directories)
            {
                DeleteDirectory(dir);
            }

            //Use the multi try delete, files are not always deleted by the time we get here.
            IOExtensions.MultiTryDirDelete(directoryPath, false);
        }
    }
}
