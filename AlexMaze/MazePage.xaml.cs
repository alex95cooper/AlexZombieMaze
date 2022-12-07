using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using AlexMazeEngine;

namespace AlexMaze
{
    public partial class MazePage : Page
    {
        private const int MazeSize = 11;

        private readonly DispatcherTimer _gameTimer = new();
        private readonly Stopwatch _stopwatch = new();

        private bool[,] _maze;
        private System.Windows.Shapes.Rectangle _player = new();

        public MazePage(string playerName)
        {
            InitializeComponent();
            _maze = new bool[MazeSize, MazeSize];
            NameBlock.Text = " " + playerName;
            ShowTime();
        }

        private void ShowTime()
        {
            TimeSpan time = new TimeSpan(0, 0, 1);
            _gameTimer.Interval = time;
            _gameTimer.IsEnabled = true;
            _gameTimer.Tick += (obj, e) => { TimeBlock.Text = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss"); };
            _stopwatch.Start();
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

            CreatePlayer();
        }



        private void CreateMazeItem(int column, int row)
        {
            BitmapImage mazeImage;
            Canvas canvas = new();
            if (_maze[row, column])
            {
                mazeImage = new(new Uri(@"Images\Ground.png", UriKind.Relative));
                canvas.Tag = "Ground";
            }
            else
            {
                mazeImage = new(new Uri(@"Images\Wall.png", UriKind.Relative));
                canvas.Tag = "Wall";
            }

            ImageBrush myImageBrush = new(mazeImage);
            canvas.Background = myImageBrush;
            AddUiElementToGreed(canvas, row, column);
        }

        private void CreatePlayer()
        {
            BitmapImage playerImage = new(new Uri(@"Images\PlayerRight.png", UriKind.Relative));    
            ImageBrush myImageBrush = new(playerImage);
            _player.Height = 30;
            _player.Width = 30;
            _player.Stroke = myImageBrush;
            AddUiElementToGreed(_player, 1, 1);
        }

        private void AddUiElementToGreed(UIElement uIElement, int row, int column)
        {
            Grid.SetRow(uIElement, row);
            Grid.SetColumn(uIElement, column);
            MazeGrid.Children.Add(uIElement);
        }
    }
}
