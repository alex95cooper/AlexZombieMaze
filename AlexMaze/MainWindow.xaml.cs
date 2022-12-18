using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Threading.Tasks;
using AlexMazeEngine.Generators;
using System.ComponentModel;
using System.Xml.Linq;
using System.Linq;

namespace AlexMaze
{
    public partial class MainWindow : Window
    {
        private const string resultFile = "result.xml";
        private const int CoinsQuantity = 10;
        private const int MazeBlockSize = 50;
        private const int MazeSize = 11;

        private readonly DispatcherTimer _gameTimer = new();
        private readonly DispatcherTimer _animationTimer = new();
        private readonly DispatcherTimer _motionTimer = new();
        private readonly DispatcherTimer _coinAddTimer = new();
        private readonly Stopwatch _stopwatch = new();
        private readonly GameInfo _gameInfo = new();
        private readonly List<Rect> _walls = new();
        private readonly List<Coin> _coins = new();
        private readonly List<Zombie> _zombies = new();

        private bool[,] _maze = new bool[MazeSize, MazeSize];
        private Player _player = default;
        private int _score;
        private bool _playerIsStep;
        private bool _zombieIsHunt;
        private bool _playerIsCaught;
        private bool _playerIsDead;

        public MainWindow()
        {
            InitializeComponent();
        }



        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            MazePanel.Visibility = Visibility.Visible;
            NameBlock.Text = NameTextBox.Text;
            LoadNewGame();
        }

        private void StatisticButton_Click(object sender, RoutedEventArgs e)
        {
            XDocument doc = XDocument.Load(resultFile);
            dataGridResult.ItemsSource = null;
            List<XElement> list = doc.Root.Elements().ToList();
            dataGridResult.ItemsSource = list;
            dataGridResult.CanUserAddRows = false;
            dataGridResult.CanUserDeleteRows = false;
            dataGridResult.DataContext = doc;
            dataGridResult.Visibility = Visibility.Visible;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyIsArrow(e))
            {
                _player.SetMove(e);
                _playerIsStep = true;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (KeyIsArrow(e))
            {
                _player.Stop();
                _playerIsStep = false;
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _gameInfo.SetCauseOfFinish(_playerIsDead);
            _gameInfo.Serialize(resultFile);
        }



        private void MotionLoop(object sender, EventArgs e)
        {
            ScoreBlock.Text = $"  Coins :  {_score}";
            if (_playerIsDead)
            {
                FinishGame();
            }

            if (_playerIsCaught == false)
            {
                TryTakeCoin();

                _player.Move();
                _player.TryMove(_walls);


                ZombieMove();
                if (_score == 2 && _zombies.Count == 1)
                {
                    CreateZombie(1);
                }
            }
        }

        private void AnimationLoop(object sender, EventArgs e)
        {
            if (_playerIsCaught == false)
            {
                foreach (var coin in _coins)
                {
                    coin.Rotate();
                }

                if (_playerIsStep)
                {
                    _player.Step();
                }

                foreach (Zombie zombie in _zombies)
                {
                    zombie.Step();
                }
            }
            else
            {
                foreach (Zombie zombie in _zombies)
                {
                    if (zombie.IsAttack)
                    {
                        _playerIsDead = zombie.Attack();
                    }
                }
            }
        }

        private void CoinAddLoop(object sender, EventArgs e)
        {
            if (_coins.Count < CoinsQuantity)
            {
                CreateAnotherCoin();
            }
        }

        private void LoadNewGame()
        {
            LoadNewMap();
            CreatePlayer();
            CreateZombie(0);
            CreateCoins();
            InitialGameTime();
            MazeCanvas.Focusable = true;
            MazeCanvas.Focus();
        }

        private void InitialGameTime()
        {
            InitialStopWatch();
            InitializeMobility();
            InitialAnimation();
            InitialCoinAdd();
        }

        private async void InitialStopWatch()
        {
            _gameTimer.Interval = new(0, 0, 1);
            _gameTimer.IsEnabled = true;
            await Task.Delay(10);
            _gameTimer.Tick += (obj, e) => { TimeBlock.Text = _stopwatch.Elapsed.ToString(@"hh\:mm\:ss"); };
            _stopwatch.Start();
        }

        private void InitializeMobility()
        {
            _motionTimer.Interval = TimeSpan.FromMilliseconds(20);
            _motionTimer.IsEnabled = true;
            _motionTimer.Tick += MotionLoop;
            _motionTimer.Start();
        }

        private void InitialAnimation()
        {
            _animationTimer.Interval = TimeSpan.FromMilliseconds(80);
            _animationTimer.IsEnabled = true;
            _animationTimer.Tick += AnimationLoop;
            _animationTimer.Start();
        }

        private void InitialCoinAdd()
        {
            _coinAddTimer.Interval = new(0, 0, 5);
            _coinAddTimer.IsEnabled = true;
            _coinAddTimer.Tick += CoinAddLoop;
        }

        private void LoadNewMap()
        {
            MazeGenerator generator = new();
            _maze = generator.GenerateNewMaze();
            AddExternalWalls();

            int blockTop = 0;
            for (int column = 0; column < _maze.GetLength(0); column++)
            {
                int blockLeft = 0;
                for (int row = 0; row < _maze.GetLength(1); row++)
                {
                    CreateMazeItem(column, row, blockLeft, blockTop);
                    blockLeft += MazeBlockSize;
                }

                blockTop += MazeBlockSize;
            }
        }

        private void CreateMazeItem(int column, int row, int blockeft, int blockTop)
        {
            Rectangle rect = new();
            rect.Width = rect.Height = MazeBlockSize;
            BitmapImage mazeImage = (_maze[column, row]) ?
                new(new Uri(@"Images\Ground.png", UriKind.Relative)) :
                new(new Uri(@"Images\Wall.png", UriKind.Relative));
            TryAddInternalWall(column, row);
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
            if (!_maze[column, row] &&
                0 < column && column < MazeSize &&
                0 < row && row < MazeSize)
            {
                _walls.Add(new Rect(
                    row * MazeBlockSize,
                    column * MazeBlockSize,
                    MazeBlockSize,
                    MazeBlockSize));
            }
        }

        private void CreatePlayer()
        {
            _player = new(@"Images\Player.png");
            System.Drawing.Point playerPosition = PositionGenerator.GetPlayerPosition(_maze);
            AddUiElementToCanvas(_player.Image,
                playerPosition.X * MazeBlockSize + Player.DistanceToWall,
                playerPosition.Y * MazeBlockSize + Player.DistanceToWall);
        }

        private void CreateZombie(int zombieNumber)
        {
            _zombies.Add(new(GetZombieImages().Item1, GetZombieImages().Item2));
            System.Drawing.Point zombiePosition = (zombieNumber == 0) ?
                PositionGenerator.GetFirstZombiePosition(_maze) :
                PositionGenerator.GetSecondZombiePosition(_maze);
            AddUiElementToCanvas(_zombies[zombieNumber].Image,
                zombiePosition.X * MazeBlockSize + Zombie.DistanceToWall,
                zombiePosition.Y * MazeBlockSize + Zombie.DistanceToWall);
            _zombies[zombieNumber].SetMove();
        }

        private void CreateCoins()
        {
            List<System.Drawing.Point> coinPositions = PositionGenerator.GetCoinsPositions(_maze, CoinsQuantity);
            for (int i = 0; i < coinPositions.Count; i++)
            {
                _coins.Add(new(GetCoinImages()));
                _coins[i].Position = coinPositions[i];
                AddUiElementToCanvas(_coins[i].Image,
                    (coinPositions[i].X * MazeBlockSize) + Coin.DistanceToWall,
                    (coinPositions[i].Y * MazeBlockSize) + Coin.DistanceToWall);
            }
        }

        private void CreateAnotherCoin()
        {
            System.Drawing.Point coinPosition = PositionGenerator.GetNewCoinPosition(_maze, _coins);
            _coins.Add(new(GetCoinImages()));
            _coins[^1].Position = coinPosition;
            AddUiElementToCanvas(_coins[^1].Image,
                    (coinPosition.X * MazeBlockSize) + Coin.DistanceToWall,
                    (coinPosition.Y * MazeBlockSize) + Coin.DistanceToWall);
        }

        private void AddUiElementToCanvas(UIElement uIElement, int left, int top)
        {
            Canvas.SetLeft(uIElement, left);
            Canvas.SetTop(uIElement, top);
            MazeCanvas.Children.Add(uIElement);
        }

        private static List<string> GetCoinImages()
        {
            List<string> coinImages = new();
            for (int i = 1; i < 5; i++)
            {
                coinImages.Add($@"Images\Coins\Coin{i}.png");
            }

            return coinImages;
        }

        private static (List<string>, List<string>) GetZombieImages()
        {
            List<string> imagesWalk = new();
            for (int i = 1; i < 11; i++)
            {
                imagesWalk.Add($@"Images\Zombie Walk\go_{i}.png");
            }

            List<string> imagesAttack = new();
            for (int i = 1; i < 8; i++)
            {
                imagesAttack.Add($@"Images\\Zombie Attack\hit_{i}.png");
            }

            return (imagesWalk, imagesAttack);
        }

        private static bool KeyIsArrow(KeyEventArgs e)
        {
            return e.Key == Key.Left || e.Key == Key.Right ||
                e.Key == Key.Up || e.Key == Key.Down;
        }

        private void TryTakeCoin()
        {
            if (_player.TryTakeCoin(_coins, out Coin deletedCoin))
            {
                _coins.Remove(deletedCoin);
                MazeCanvas.Children.Remove(deletedCoin.Image);
                _score++;
                if (_score >= 5)
                {
                    _zombieIsHunt = true;
                    foreach (Zombie zombie in _zombies)
                    {
                        zombie.Accelerate();
                    }
                }
            }
        }

        private void ZombieMove()
        {
            foreach (Zombie zombie in _zombies)
            {
                zombie.Move();
                _playerIsCaught = (!_playerIsCaught) && zombie.TryCatchPlayer(_player);
                if (_zombieIsHunt)
                {
                    int direction = PathGenerator.GetDirection(_maze, zombie, _player);
                    zombie.Hunt(direction);
                    zombie.TryMoveWithHunt(_walls);
                }
                else
                {
                    zombie.TryMove(_walls);
                }
            }
        }

        private void FinishGame()
        {
            _gameInfo.SetCauseOfFinish(_playerIsDead);
            _gameInfo.Serialize(resultFile);
            MazePanel.Visibility = Visibility.Collapsed;
        }
    }
}
