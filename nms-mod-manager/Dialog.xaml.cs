using System.Windows;

namespace nms_mod_manager
{
    /// <summary>
    /// Logica di interazione per Enabled.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
