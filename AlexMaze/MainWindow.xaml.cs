using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AlexMazeEngine.Generators;
using AlexMazeEngine;
using AlexMazeEngine.Humanoids;

namespace AlexMaze
{
    public partial class MainWindow : Window
    {
        private const int CoinsQuantity = 10;
        private const int MazeBlockSize = 50;
        private const int MiddleLevelCoins = 3;
        private const int HardLevelCoins = 6;

        private readonly GameTimer _timer;
        private readonly MapBuilder _mapBuilder = new();

        private EntityBuilder _entityBuilder;
        private List<Coin> _coins;
        private List<Zombie> _zombies;
        private Player _player;
        private GameInfo _gameInfo;
        private int _score;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new(this);
            NameTextBox.Text = GameInfo.GetLastPlayerName();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameInfo = new(NameTextBox.Text);
            MazePanel.Visibility = Visibility.Visible;
            MenuPanel.Visibility = Visibility.Collapsed;
            NameBlock.Text = NameTextBox.Text;
            LoadNewGame();
        }

        private void StatisticButton_Click(object sender, RoutedEventArgs e)
        {
            dataGridResult = GameInfo.GetStatistic();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                FinishGame();
            }
            else if (KeyIsArrow(e) && _player != null)
            {
                _player.SetMove(e);
                _player.State = PlayerState.Step;
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (KeyIsArrow(e))
            {
                _player.Stop();
                _player.State = PlayerState.Stop;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MazePanel.Visibility == Visibility.Visible)
            {
                _gameInfo.SetLastInfo(_score, TimeBlock.Text, PlayerState.Dead);
                _gameInfo.Serialize();
            }
        }

        public void MotionLoop(object sender, EventArgs e)
        {
            if (_player.State == PlayerState.Dead)
            {
                FinishGame();
            }

            if (_player.State != PlayerState.Caught)
            {
                TryTakeCoin();
                _player.Move();
                _player.TryMove(_mapBuilder.Walls);
                ZombieMove();
                if (_score == MiddleLevelCoins && _zombies.Count == 1)
                {
                    _entityBuilder.CreateZombie(1);
                }
            }
        }

        public void AnimationLoop(object sender, EventArgs e)
        {
            if (_player.State == PlayerState.Caught)
            {
                AttackPlayer();
            }
            else
            {
                Coin.Rotate(_coins);
                Zombie.Step(_zombies);
                if (_player.State == PlayerState.Step)
                {
                    _player.Step();
                }
            }
        }

        public void CoinAddLoop(object sender, EventArgs e)
        {
            if (_coins.Count < CoinsQuantity)
            {
                _entityBuilder.CreateAnotherCoin();
            }
        }

        private void LoadNewGame()
        {
            MazeCanvas.Children.Add(_mapBuilder.Canvas);
            CreateEntities();
            _timer.Start();
            MazeCanvas.Focusable = true;
            MazeCanvas.Focus();
        }

        private void CreateEntities()
        {
            _entityBuilder = new EntityBuilder(_mapBuilder.Maze, MazeBlockSize, CoinsQuantity);
            MazeCanvas.Children.Add(_entityBuilder.Canvas);
            _player = _entityBuilder.Player;
            _zombies = _entityBuilder.Zombies;
            _coins = _entityBuilder.Coins;
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
                ScoreBlock.Text = $"  Coins :  {_score}";
                Canvas canvas = (Canvas)MazeCanvas.Children[1];
                canvas.Children.Remove(deletedCoin.Image);
                _coins.Remove(deletedCoin);
                _score++;
                foreach (Zombie zombie in _zombies)
                {
                    if (_score == HardLevelCoins)
                    {
                        zombie.State = ZombieState.Hunt;
                    }
                    else if (_score > HardLevelCoins)
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
                zombie.TryCatchPlayer(_player);               
                if (zombie.State == ZombieState.Hunt)
                {
                    MoveDirection direction = PathGenerator.GetDirection(_mapBuilder.Maze, zombie, _player);
                    zombie.Hunt(direction);
                }

                zombie.Move();
                zombie.TryMove(_mapBuilder.Walls);
            }
        }

        private void AttackPlayer()
        {
            foreach (Zombie zombie in _zombies)
            {
                if (zombie.State == ZombieState.Attack)
                {
                    zombie.Attack();
                    _player.State = (PlayerState)zombie.State;
                }
            }
        }

        private void FinishGame()
        {
            _timer.Stop();
            MainWindow newWindow = new();
            Application.Current.MainWindow = newWindow;
            newWindow.Show();
            this.Close();
        }
    }
}
