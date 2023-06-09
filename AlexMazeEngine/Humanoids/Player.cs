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
            LookDirection = LookDirection.Right;
        }

        public PlayerState State { get; set; }

        public void SetMove(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    MoveDirection = MoveDirection.Left;
                    TryMakeTurn(LookDirection.Left);
                    break;
                case Key.Right:
                    MoveDirection = MoveDirection.Right;
                    TryMakeTurn(LookDirection.Right);
                    break;
                case Key.Up:
                    MoveDirection = MoveDirection.Up;
                    break;
                case Key.Down:
                    MoveDirection = MoveDirection.Down;
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
            MoveDirection = MoveDirection.None;
            SetImage(ImagePath);
        }

        public void Step()
        {
            StepCounter++;
            SetImage(ImagePath, (int)Width * StepCounter);
            StepCounter = (StepCounter == StepCount) ? 0 : StepCounter;
        }

        private void TryMakeTurn(LookDirection playerLook)
        {
            if (playerLook == LookDirection.Left && LookDirection != LookDirection.Left)
            {
                LookDirection = LookDirection.Left;
                Image.FlowDirection = FlowDirection.RightToLeft;
            }
            else if (playerLook == LookDirection.Right && LookDirection != LookDirection.Right)
            {
                LookDirection = LookDirection.Right;
                Image.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        public override void AvoidTouchingWall()
        {
            MoveDirection = MoveDirection.None;
        }
    }
}
