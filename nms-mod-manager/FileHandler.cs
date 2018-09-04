using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace nms_mod_manager
{
    class FileHandler
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string disablemods = AppDomain.CurrentDomain.BaseDirectory + "GAMEDATA\\PCBANKS\\disablemods.txt";

        /// <summary>
        /// Sanity checks exeecuted when starting up or refreshing the application.
        /// </summary>
        public int FolderCheck()
        {
            if (Directory.Exists(path + "GAMEDATA\\PCBANKS") == false || Directory.Exists(path + "Binaries") == false)
            {
                //Game folders not detected
                return 0;
            }
            else
            {
                //Check if the mods folders exist, if not create them
                Directory.CreateDirectory($"{path}GAMEDATA\\PCBANKS\\MODS");
                Directory.CreateDirectory($"{path}mods");

                //Check if the lock exists
                if (File.Exists(path + "GAMEDATA\\PCBANKS\\disablemods.txt"))
                {
                    //Tell the Window to display a dialog
                    return 2;
                }
                else
                {
                    // If 'mods' is empty and mods are present in 'GAMEDATA\PCBANKS\MODS\
                    if (IsDirectoryEmpty($"{path}mods\\") && IsDirectoryEmpty($"{path}GAMEDATA\\PCBANKS\\MODS\\") == false)
                    {
                        //Copy the mods files from the game folder to the 'mods' folder
                        CopyAllFiles();
                        return 3;
                    }
                    else
                    {
                        //Tell the Window it's all good
                        return 1;
                    }
                }
            }
        }

        /// <summary>
        /// Copies all the mods from the game folder to the 'mods' folder
        /// </summary>
        public void CopyAllFiles()
        {
            DirectoryInfo enableDir = new DirectoryInfo($"{path}GAMEDATA\\PCBANKS\\MODS");
            FileInfo[] enableFiles = enableDir.GetFiles("*.pak");
            foreach (FileInfo file in enableFiles)
            {
                if (File.Exists($"{path}mods\\{file.Name}") == false)
                {
                    Console.WriteLine($"{file.FullName} has been copied.");
                    File.Copy(file.FullName, $"{path}mods\\{file.Name}");
                }
            }
        }

        /// <summary>
        /// Copies the specified mod inside the game directory.
        /// </summary>
        /// <param name="name">
        /// A string containing the mod's filename.
        /// </param>
        public void Copy(string name)
        {
            string modPath = path + "mods\\" + name;
            string storePath = path + "GAMEDATA\\PCBANKS\\MODS\\" + name;
            if (File.Exists(storePath) == false)
            {
                File.Copy(modPath, storePath);
            }
        }

        /// <summary>
        /// Deletes the specified mod from the game directory.
        /// </summary>
        /// <param name="name">
        /// A string containing the mod's filename.
        /// </param>
        public void Delete(string name)
        {
            string storePath = path + "GAMEDATA\\PCBANKS\\MODS\\" + name;
            if (File.Exists(storePath = path + "GAMEDATA\\PCBANKS\\MODS\\" + name)) 
            {
                File.Delete(storePath);
            }
        }
        
        /// <summary>
        /// Checks if a given directory is empty.
        /// </summary>
        /// <param name="path">Absolute path to the given directory.</param>
        /// <returns></returns>
        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}
