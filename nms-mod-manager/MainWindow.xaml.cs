using System;
using System.IO;
using System.Windows;

namespace nms_mod_manager
{
    /// <summary>
    /// MainWindow.xaml interation logic
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;

        string disablemods = AppDomain.CurrentDomain.BaseDirectory + "GAMEDATA\\PCBANKS\\disablemods.txt";

        FileHandler fs = new FileHandler();
        HexConverter cc = new HexConverter();
        AskDialog ask = new AskDialog();

        public MainWindow()
        {
            InitializeComponent();
            Check();
            RefreshList(1);
            RefreshList(2);

            //Allow mods to be dropped inside the TOP list
            modList.AllowDrop = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        //Start No Man's Sky
        private void StartNMS(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("steam://run/275850");
            this.Close();
        }

        /// <summary>
        /// Handles FolderCheck() result codes and displays a message box accordingly.
        /// </summary>
        /// <param name="owner">Sets whether the dialog box should be centered in front of the parent window.</param>
        private void Check(bool owner = false)
        {
            if (fs.FolderCheck() == 3)
            {
                Dialog("Modfiles have been copied to the 'mods' folder.", owner);
            }
            else
            if (fs.FolderCheck() == 1)
            {
                Dialog("Mods detected successfully.", owner);
            }
            else
            if (fs.FolderCheck() == 2)
            {
                ask.Dialog("Lock detected! Do you want\nto delete 'disablemods.txt'?");
            }
            else
            if (fs.FolderCheck() == 0)
            {
                Dialog("This is not a valid\nNo Man's Sky installation!", owner);
                Close();
            }

            if (File.Exists(disablemods) == true)
            {
                modStatus.Content = "Disabled";
                modStatus.Foreground = cc.Color("#FFA500");
            }
            else
            {
                modStatus.Content = "Enabled";
                modStatus.Foreground = cc.Color("#B7494E");
            }
        }

        /// <summary>
        /// Refreshes the specified tab.
        /// </summary>
        /// <param name="tab">
        /// <para>1 = Top list</para>
        /// <para>2 = Bottom list</para>
        /// </param>
        private void RefreshList(int tab)
        {
            if (tab == 1)
            {
                modList.Items.Clear();
                DirectoryInfo modDir = new DirectoryInfo(path + "mods\\");
                FileInfo[] modFiles = modDir.GetFiles("*.pak");
                foreach (FileInfo file in modFiles)
                {
                    Console.WriteLine($"{@file.FullName} has been detected!");
                    modList.Items.Add(file.Name);
                }
            }

            if (tab == 2)
            {
                enableList.Items.Clear();
                DirectoryInfo enableDir = new DirectoryInfo(path + "GAMEDATA\\PCBANKS\\MODS");
                FileInfo[] enableFiles = enableDir.GetFiles("*.pak");
                foreach (FileInfo file in enableFiles)
                {
                    enableList.Items.Add(file.Name);
                    Console.WriteLine($"{@file.FullName} has been detected!");
                }
            }
        }

        /// <summary>
        /// Creates a dynamic dialog box with the specified text.
        /// </summary>
        /// <param name="content">The text to display.</param>
        /// <param name="owner">Sets whether the dialog box should be centered in front of the parent window.</param>
        /// <remarks>'owner' needs to be set to false when the dialog is shown before the MainWindow!</remarks>
        private void Dialog(string content, bool owner = false)
        {
            Dialog dialog = new Dialog();
            if (owner)
            {
                dialog.Owner = this;
            }
            dialog.labelDialog.Content = content;
            dialog.ShowDialog();
        }

        private void ModEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void ModDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file) == ".pak")
                    {
                        string fileName = Path.GetFileName(file);
                        try
                        {
                            File.Copy(file, $"{path}mods\\{fileName}");
                            Dialog($"{fileName} has been imported.", true);
                        }
                        catch (IOException)
                        {
                            if (File.Exists(file))
                            {
                                Dialog($"{fileName} is already in the mods folder!", true);
                            }
                            else
                            {
                                Dialog($"Failed to copy {fileName}!", true);
                            }
                        }
                    }
                    else
                    {
                        Dialog("Unknown file!", true);
                    }
                }
                RefreshList(1);
            }
        }

        private void Enable(object sender, RoutedEventArgs e)
        {
            if (File.Exists(disablemods) == true)
            {
                try
                {
                    File.Delete(disablemods);

                    modStatus.Foreground = cc.Color("#B7494E");
                    modStatus.Content = "Enabled";
                    Dialog("MODS ENABLED", true);
                }
                catch (IOException)
                {
                    Dialog("Failed to remove lock!", true);
                }
            }
        }


        private void Disable(object sender, RoutedEventArgs e)
        {
            if (File.Exists(disablemods) == false)
            {
                try
                {
                    File.Create(disablemods);

                    modStatus.Foreground = cc.Color("#FFA500");
                    modStatus.Content = "Disabled";
                    Dialog("MODS DISABLED", true);
                }
                catch (IOException)
                {
                    Dialog("Failed to add lock!", true);
                }
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            //Rescan folders
            Check(true);
            //Add mods to list
            RefreshList(1);
            RefreshList(2);
        }

        /// <summary>
        /// Enables the selected mod(s).
        /// </summary>
        /// <remarks>This event will copy the selected mods from 'GAMEDATA\PCBANKS\MODS\'.</remarks>
        private void EnableSelected(object sender, RoutedEventArgs e)
        {
            //Check if there are no items selected.
            if (modList.SelectedItems.Count == 0)
            {
                Dialog("No mod file selected.", true);
            }
            else
            {
                int fCount = modList.SelectedItems.Count;
                for (int i = 0; i < fCount; i++)
                {
                    string name = modList.SelectedItems[i].ToString();

                    //Check if the file has already been copied once.
                    if (File.Exists($"{path}GAMEDATA\\PCBANKS\\MODS\\{name}"))
                    {
                        Dialog($"{name} is already enabled.", true);
                    }
                    else
                    {
                        try
                        {
                            //Copy the file
                            fs.Copy($"{@modList.SelectedItems[i].ToString()}");
                            Dialog($"{name} has been enabled.", true);
                            Console.WriteLine($"{path}mods\\{name} has been copied.");
                        }
                        catch (IOException)
                        {
                            Dialog($"Cannot enable {name}!", true);
                        }
                    }
                }
                RefreshList(2);
            }
        }

        /// <summary>
        /// Disables the selected mod(s).
        /// </summary>
        /// <remarks>This event will delete the selected mods from 'GAMEDATA\PCBANKS\MODS\'.</remarks>
        private void DisableSelected(object sender, RoutedEventArgs e)
        {
            //Check if there are no items selected.
            if (enableList.SelectedItems.Count == 0)
            {
                Dialog("No mod file selected.", true);
            }
            else
            {
                int fCount = enableList.SelectedItems.Count;
                for (int i = 0; i < fCount; i++)
                {
                    string name = enableList.SelectedItems[i].ToString();
                    try
                    {
                        fs.Delete($"{enableList.SelectedItems[i].ToString()}");
                        Dialog($"{name} has been disabled.", true);
                        Console.WriteLine($"{path}GAMEDATA\\PCBANKS\\MODS\\{name} has been deleted.");
                    }
                    catch (IOException)
                    {
                        Dialog($"Cannot disable {name}!", true);
                    }
                }
                RefreshList(2);
            }
        }

        private void DeleteShaders(object sender, RoutedEventArgs e)
        {
            DirectoryInfo enableDir = new DirectoryInfo(path + "GAMEDATA\\SHADERCACHE");
            FileInfo[] enableFiles = enableDir.GetFiles();
            foreach (FileInfo file in enableFiles)
            {
                File.Delete(file.FullName);
                Console.WriteLine($"{@file.FullName} has been deleted!");
            }
            Dialog("SHADERCACHE has been deleted.", true);
        }
    }
}
