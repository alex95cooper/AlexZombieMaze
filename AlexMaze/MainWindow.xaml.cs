using System.Windows;

namespace AlexMaze
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new MenuPage();
            
        }

        private void MainFrame_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            MainFrame.Focusable = false;
        }
    }
}
