using System;
using System.IO;
using System.Windows;

namespace nms_mod_manager
{
    /// <summary>
    /// Logica di interazione per AskBox.xaml
    /// </summary>
    public partial class AskDialog : Window
    {
        FileHandler fs = new FileHandler();
        Dialog dialog = new Dialog();

        string disablemods = AppDomain.CurrentDomain.BaseDirectory + "GAMEDATA\\PCBANKS\\disablemods.txt";

        public AskDialog()
        {
            InitializeComponent();
        }
        
        private void noClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Dialog(string content)
        {
            AskDialog askDialog = new AskDialog();
            askDialog.askDialog.Content = content;
            askDialog.ShowDialog();
        }

        private void yesClick(object sender, RoutedEventArgs e)
        {
            File.Delete(disablemods);
            this.Close();
        }
    }
}
