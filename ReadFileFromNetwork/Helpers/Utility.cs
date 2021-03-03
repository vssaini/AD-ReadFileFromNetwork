using System;
using System.Collections;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;

namespace ReadFileFromNetwork.Helpers
{
    /// <summary>
    /// Provide helper functions.
    /// </summary>
    class Utility
    {
        [DllImport("shlwapi.dll")]
        public static extern bool PathIsNetworkPath(string pszPath);

        /// <summary>
        /// Read shared file from UNC path.
        /// </summary>
        /// <param name="networkName">The network name as \\192.168.1.202\data</param>
        /// <param name="networkFile">The network file as \\192.168.1.202\data\AboutMe.txt</param>
        /// <returns>Return value from file</returns>
        public static string ReadShareFile(string networkName, string networkFile)
        {
            string fileText;
            if (PathIsNetworkPath(networkFile))
            {
                var nw = GetNetworkCredential();
                using (new NetworkConnection(networkName, nw))
                {
                    // Read file
                    if (File.Exists(networkFile))
                    {
                        fileText = File.ReadAllText(networkFile);
                    }
                    else
                    {
                        throw new Exception(string.Format("File '{0}' not found.", networkFile));
                    }
                }
            }
            else
            {
                // Read file
                if (File.Exists(networkFile))
                {
                    fileText = File.ReadAllText(networkFile);
                }
                else
                {
                    throw new Exception(string.Format("File '{0}' not found.", networkFile));
                }
            }

            return fileText;
        }

        /// <summary>
        /// Get UNC path from the path passed
        /// </summary>
        /// <param name="filePath">The file path that need to be evaluated</param>
        /// <returns>Return UNC path</returns>
        public static string GetUNCPath(string filePath)
        {
            var path = filePath.TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
            var dirInfo = new DirectoryInfo(path);
            var root = dirInfo.Root.FullName.TrimEnd('\\');

            if (!root.StartsWith(@"\\"))
            {
                var mo = new ManagementObject
                {
                    Path = new ManagementPath(string.Format("Win32_LogicalDisk='{0}'", root))
                };

                // Get drive type and set path as per that
                var driveType = Convert.ToUInt32(mo["DriveType"]);

                switch (driveType)
                {
                    case 0: // Unknown
                    case 1: // No root directory
                    case 2: // Removable disk
                    case 5: // Compact Disk
                    case 6: // RAM Disk
                        break;

                    case 3: // Local disk
                         // root = string.Format(@"\\{0}\{1}$\", Dns.GetHostName(), root.TrimEnd(':'));
                        break;

                    case 4: // Network Drive
                        root = Convert.ToString(mo["ProviderName"]);
                        filePath = Recombine(root, dirInfo);
                        break;
                }
            }

            return filePath;
        }

        #region Helpers

        private static string Recombine(string root, DirectoryInfo dirInfo)
        {
            var stack = new Stack();
            while (dirInfo.Parent != null)
            {
                stack.Push(dirInfo.Name);
                dirInfo = dirInfo.Parent;
            }

            while (stack.Count > 0)
            {
                root = Path.Combine(root, (string)stack.Pop());
            }
            return root;
        }

        /// <summary>
        /// Get network credential object.
        /// </summary>
        private static NetworkCredential GetNetworkCredential()
        {
            var nw = new NetworkCredential
            {
                Domain = "domain",
                UserName = "Administrator",
                Password = "Pass99"
            };

            return nw;
        }

        #endregion

    }
}
