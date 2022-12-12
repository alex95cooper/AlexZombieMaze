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
        private Zombie _zombie1;
        private Zombie? _zombie2;
        private bool _playerIsStep;
        private int _score;

        public MazePage(string playerName)
        {
            InitializeComponent();
            _player = new(@"Images\Player.png");
            (List<string>, List<string>) zombieMotions = GetZombiekMotions();
            _zombie1 = new(zombieMotions.Item1, zombieMotions.Item2);
            _zombie2 = null;
            _maze = new bool[MazeSize, MazeSize];
            NameBlock.Text = " " + playerName;
            DisplayGameTime();
            LoadNewGame();

        }

        private void MazeCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyIsArrow(e))
            {
                _player.SetMove(e);
                _playerIsStep = true;
            }
        }

        private void MazeCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyIsArrow(e))
            {
                _player.Stop();
                _playerIsStep = false;
            }
        }

        private static bool KeyIsArrow(KeyEventArgs e)
        {
            return e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Up || e.Key == Key.Down;
        }

        private void MotionLoop(object? sender, EventArgs e)
        {
            if (_playerIsStep)
            {
                _player.Step();
            }



            _player.Move();
            _player.TryMove(_walls);

            _zombie1.Step();
            _zombie1.Move();
            _zombie1.TryMove(_walls);


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
            CreateZombie();
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
            AddUiElementToCanvas(_player.Image, 
                MazeBlockSize + Player.StopDistance, 
                MazeBlockSize + Player.StopDistance);
        }

        private void CreateZombie()
        {
            AddUiElementToCanvas(_zombie1.Image, (MazeBlockSize * 9) + 1, (MazeBlockSize * 9) + 1);
            _zombie1.SetMove();
        }

        private (List<string>, List<string>) GetZombiekMotions()
        {
            List<string> imagesWalk = new();
            for (int i = 1; i < 11; i++)
            {
                imagesWalk.Add($@"Images\Zombie Walk\go_{i}.png");
            }

            List<string> imagesAttack = new();
            for (int i = 1; i < 11; i++)
            {
                imagesWalk.Add($@"Images\\Zombie Attack\hit_{i}.png");
            }

            return (imagesWalk, imagesAttack);
        }

        private void AddUiElementToCanvas(UIElement uIElement, int left, int top)
        {
            Canvas.SetLeft(uIElement, left);
            Canvas.SetTop(uIElement, top);
            MazeCanvas.Children.Add(uIElement);
        }
    }
}
