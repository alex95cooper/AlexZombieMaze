using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        private const int MazeBlockSize = 44;
        private const int MazeSize = 11;

        private readonly DispatcherTimer _motionTimer = new();
        private readonly DispatcherTimer _gameTimer = new();
        private readonly Stopwatch _stopwatch = new();
        private readonly List<Rect> _walls = new();

        private bool[,] _maze;
        private Player _player;

        private int _score;

        public MazePage(string playerName)
        {
            InitializeComponent();
            _player = new(@"Images\PlayerRight.png");
            _maze = new bool[MazeSize, MazeSize];
            NameBlock.Text = " " + playerName;
            DisplayGameTime();
            LoadNewGame();

        }

        private void MazeCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            _player.SetMove(e);
        }

        private void MazeCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            _player.Stop();
        }
        private void MotionLoop(object? sender, EventArgs e)
        {
            _player.Move();
            _player.TryMove(_walls);

        }

        private void DisplayGameTime()
        {
            TimeSpan gameTime = new(0, 0, 1);
            _gameTimer.Interval = gameTime;
            _gameTimer.IsEnabled = true;
            _gameTimer.Tick += (obj, e) => { TimeBlock.Text = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss"); };
            _stopwatch.Start();
        }

        private void LoadNewGame()
        {
            LoadNewMap();
            CreatePlayer();
            InitializeMobility();
            MazeCanvas.Focusable = true;
            MazeCanvas.Focus();
        }

        private void InitializeMobility()
        {
            _motionTimer.Interval = TimeSpan.FromMilliseconds(15);
            _motionTimer.IsEnabled = true;
            _motionTimer.Tick += MotionLoop;
            _motionTimer.Start();
        }

        private void LoadNewMap()
        {
            MazeGenerator generator = new();
            _maze = generator.GenerateNewMaze();
            AddExternalWalls();

            int blockTop = 0;
            for (int row = 0; row < _maze.GetLength(0); row++)
            {
                int blockleft = 0;
                for (int column = 0; column < _maze.GetLength(1); column++)
                {
                    CreateMazeItem(column, row, blockleft, blockTop);
                    blockleft += MazeBlockSize;
                }

                blockTop += MazeBlockSize;
            }
        }

        private void CreateMazeItem(int column, int row, int blockeft, int blockTop)
        {
            Rectangle rect = new();
            rect.Width = rect.Height = MazeBlockSize;
            BitmapImage mazeImage;
            if (_maze[column, row])
            {
                mazeImage = new(new Uri(@"Images\Ground.png", UriKind.Relative));
            }
            else
            {
                mazeImage = new(new Uri(@"Images\Wall.png", UriKind.Relative));
                TryAddInternalWall(column, row);
            }

            ImageBrush myImageBrush = new(mazeImage);
            rect.Fill = myImageBrush;
            AddUiElementToCanvas(rect, blockeft, blockTop);
        }

        private void AddExternalWalls()
        {
            _walls.Add(new Rect(0, 0, MazeBlockSize * MazeSize, MazeBlockSize));
            _walls.Add(new Rect(0, MazeBlockSize, MazeBlockSize, MazeBlockSize * (MazeSize - 2)));
            _walls.Add(new Rect((MazeBlockSize * MazeSize) - 1, MazeBlockSize, MazeBlockSize, MazeBlockSize * (MazeSize - 2)));
            _walls.Add(new Rect(0, (MazeBlockSize * MazeSize) - 1, MazeBlockSize * MazeSize, MazeBlockSize));
        }

        private void TryAddInternalWall(int column, int row)
        {
            if (0 < column && column < MazeSize && 0 < row && row < MazeSize)
            {
                _walls.Add(new Rect(
                    column * MazeBlockSize,
                    row * MazeBlockSize,
                    MazeBlockSize,
                    MazeBlockSize));
            }
        }

        private void CreatePlayer()
        {
            AddUiElementToCanvas(_player.Image, MazeBlockSize + 2, MazeBlockSize + 2);
        }

        private void AddUiElementToCanvas(UIElement uIElement, int left, int top)
        {
            Canvas.SetLeft(uIElement, left);
            Canvas.SetTop(uIElement, top);
            MazeCanvas.Children.Add(uIElement);
        }
    }
}
