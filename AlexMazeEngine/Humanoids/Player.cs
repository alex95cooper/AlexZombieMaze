using AlexMazeEngine.Humanoids;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AlexMazeEngine
{
    public class Player : Humanoid
    {
        private const int StepCount = 7;
        private const double MinSpeed = 2;
        private const double PlayerWidth = 12;

        public Player(string imagepath)
            : base(imagepath, PlayerWidth, MinSpeed)
        {
            _lookDirection = LookDirection.Right;
        }

        public PlayerState State { get; set; }

        public void SetMove(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _moveDirection = MoveDirection.Left;
                    TryMakeTurn(LookDirection.Left);
                    break;
                case Key.Right:
                    _moveDirection = MoveDirection.Right;
                    TryMakeTurn(LookDirection.Right);
                    break;
                case Key.Up:
                    _moveDirection = MoveDirection.Up;
                    break;
                case Key.Down:
                    _moveDirection = MoveDirection.Down;
                    break;
            }
        }

        public bool TryTakeCoin(List<Coin> coins, out Coin deletedCoin)
        {
            deletedCoin = default;
            Rect playerHitBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            foreach (var coin in coins)
            {
                Rect CoinTakeBox = new(Canvas.GetLeft(coin.Image), Canvas.GetTop(coin.Image), Coin.Width, Coin.Height);
                if (playerHitBox.IntersectsWith(CoinTakeBox))
                {
                    deletedCoin = coin;
                    coins.Remove(coin);
                    return true;
                }
            }

            return false;
        }

        public void Stop()
        {
            _moveDirection = MoveDirection.None;
            SetImage(_imagePath);
        }

        public void Step()
        {
            _stepCounter++;
            SetImage(_imagePath, (int)Width * _stepCounter);
            _stepCounter = (_stepCounter == StepCount) ? 0 : _stepCounter;
        }

        private void TryMakeTurn(LookDirection playerLook)
        {
            if (playerLook == LookDirection.Left && _lookDirection != LookDirection.Left)
            {
                _lookDirection = LookDirection.Left;
                Image.FlowDirection = FlowDirection.RightToLeft;
            }
            else if (playerLook == LookDirection.Right && _lookDirection != LookDirection.Right)
            {
                _lookDirection = LookDirection.Right;
                Image.FlowDirection = FlowDirection.LeftToRight;
            }
        }
    }
}
