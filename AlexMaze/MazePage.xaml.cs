using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AlexMazeEngine;

namespace AlexMaze
{
    public partial class MazePage : Page
    {
        private const int MazeSize = 11;

        private bool[,] _maze;

        public MazePage()
        {
            InitializeComponent();
            _maze = new bool[MazeSize, MazeSize];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MazeGenerator generator = new MazeGenerator();
            _maze = generator.GenerateNewMaze();
            for (int row = 0; row < _maze.GetLength(0); row++)
            {
                MazeGrid.RowDefinitions.Add(new RowDefinition());
                for (int column = 0; column < _maze.GetLength(1); column++)
                {
                    if (row == 0)
                    {
                        MazeGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                        
                    CreateMazeItem(column, row);
                }
            }
        }

        private void CreateMazeItem(int column, int row)
        {
            BitmapImage theImage;
            Canvas canvas = new();
            if (_maze[row, column])
            {
                theImage = new(new Uri(@"Images\Ground.png", UriKind.Relative));
            }
            else
            {
                theImage = new(new Uri(@"Images\Wall.png", UriKind.Relative));
            }

            ImageBrush myImageBrush = new(theImage);
            canvas.Background = myImageBrush;
            AddUiElementToGreed(canvas, row, column);
        }

        private void AddUiElementToGreed(UIElement uIElement, int row, int column)
        {
            Grid.SetRow(uIElement, row);
            Grid.SetColumn(uIElement, column);
            MazeGrid.Children.Add(uIElement);
        }
    }
}
