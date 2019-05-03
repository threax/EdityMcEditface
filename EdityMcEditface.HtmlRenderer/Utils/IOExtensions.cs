using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace EdityMcEditface.Utils
{
    public static class IOExtensions
    {
        /// <summary>
        /// Try mulitplie times to delete a directory since this fails a lot.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="recursive">True to do recursive delete, false otherwise.</param>
        public static void MultiTryDirDelete(String dir, bool recursive)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, recursive);
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(dir, recursive);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(100); //Small timeout if we got this far
                        Directory.Delete(dir, recursive); //Last one will throw if needed
                    }
                }
            }
        }

        /// <summary>
        /// Try mulitplie times to delete a directory since this fails a lot.
        /// </summary>
        /// <param name="file"></param>
        public static void MultiTryFileDelete(String file)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(100); //Small timeout if we got this far
                        File.Delete(file); //Last one will throw if needed
                    }
                }
            }
        }
    }
}
