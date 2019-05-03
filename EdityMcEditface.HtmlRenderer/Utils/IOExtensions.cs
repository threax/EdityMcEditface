﻿using System;
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
        public static void MultiTryDirDelete(String dir)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch (Exception)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(100); //Small timeout if we got this far
                        Directory.Delete(dir, true); //Last one will throw if needed
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
