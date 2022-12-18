﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine
{
    public class Player : IHumanoid
    {
        public const double ImageScale = 0.5;
        public const double Height = 32;
        public const double Width = 12;
        public const int StepCount = 7;
        public const int Speed = 2;
        public const int DistanceToWall = 1;

        private readonly string _imagePath;

        private int _playersLook;
        private int _moveDirection;
        private int _stepCounter;

        public Player(string imagepath)
        {
            _imagePath = imagepath;
            Image = new Image();
            _playersLook = (int)LookDirection.Right;
            SetImage(0);
        }

        public Image Image { get; }

        private void SetImage(int rectX)
        {
            BitmapImage playerImage = new(new Uri(_imagePath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(playerImage, new ScaleTransform(ImageScale, ImageScale));
            CroppedBitmap frame = new(reducedBitmap, new Int32Rect(rectX, 0, (int)Width, (int)Height));
            Image.Source = frame;
            Image.Width = frame.Width;
            Image.Height = frame.Height;
        }

        public void SetMove(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _moveDirection = (int)MoveDirection.Left;
                    TryMakeTurn((int)LookDirection.Left);
                    break;
                case Key.Right:
                    _moveDirection = (int)MoveDirection.Right;
                    TryMakeTurn((int)LookDirection.Right);
                    break;
                case Key.Up:
                    _moveDirection = (int)MoveDirection.Up;
                    break;
                case Key.Down:
                    _moveDirection = (int)MoveDirection.Down;
                    break;
            }
        }

        public void Move()
        {
            switch (_moveDirection)
            {
                case (int)MoveDirection.Left:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) - Speed);
                    break;
                case (int)MoveDirection.Right:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) + Speed);
                    break;
                case (int)MoveDirection.Up:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) - Speed);
                    break;
                case (int)MoveDirection.Down:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) + Speed);
                    break;
            }
        }

        public void TryMove(List<Rect> walls)
        {
            Rect playerHitBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            foreach (var block in walls)
            {
                if (playerHitBox.IntersectsWith(block))
                {
                    switch (_moveDirection)
                    {
                        case (int)MoveDirection.Left:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) + (Speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Right:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) - (Speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Up:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) + (Speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Down:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) - (Speed + DistanceToWall));
                            break;
                    }

                    _moveDirection = (int)MoveDirection.None;
                }
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
            _moveDirection = (int)MoveDirection.None;
            SetImage(0);
        }

        public void Step()
        {
            _stepCounter++;
            SetImage((int)Width * _stepCounter);
            _stepCounter = (_stepCounter == StepCount) ? 0 : _stepCounter;
        }

        private void TryMakeTurn(int playerLook)
        {
            if (playerLook == (int)LookDirection.Left && _playersLook != (int)LookDirection.Left)
            {
                _playersLook = (int)LookDirection.Left;
                Image.FlowDirection = FlowDirection.RightToLeft;
            }
            else if (playerLook == (int)LookDirection.Right && _playersLook != (int)LookDirection.Right)
            {
                _playersLook = (int)LookDirection.Right;
                Image.FlowDirection = FlowDirection.LeftToRight;
            }
        }
    }
}