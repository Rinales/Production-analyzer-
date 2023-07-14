using System.Windows;
using System.Windows.Controls;

namespace Production_analyze
{
    /// <summary>
    /// Interakční logika pro LoadData.xaml
    /// </summary>
    public partial class LoadData : Page
    {
        public LoadData()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Main.Content = new MainWindow();
        }


    }
}
