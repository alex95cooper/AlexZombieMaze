using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AlexMaze
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = NameTextBox.Text;
            NavigationService.Navigate(new MazePage(playerName));
        }
    }
}
