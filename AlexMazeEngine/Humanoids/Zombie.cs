﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine
{
    public class Zombie : IHumanoid
    {
        public const double ImageScale = 0.5;
        public const double Height = 32;
        public const double Width = 20;
        public const double MinSpeed = 0.5;
        public const double MaxSpeed = 10;
        public const int AttackCount = 7;
        public const int StepCount = 10;
        public const int DistanceToWall = 1;

        private readonly List<string> _imagesWalk;
        private readonly List<string> _imagesAttack;
        private readonly string _imagePath;

        private double _speed;
        private int _zombieLook;
        private int _moveDirection;
        private int _lastSuccessfulDirection;
        private int _stepCounter;
        private int _attackCounter;
        private bool _zombieWalksHorizontally;

        public Zombie(List<string> imagesWalk, List<string> imagesAttack)
        {
            _imagesWalk = imagesWalk;
            _imagesAttack = imagesAttack;
            _imagePath = imagesWalk[0];
            Image = new Image();
            _speed = 0.5;
            _zombieWalksHorizontally = true;
            _zombieLook = (int)LookDirection.Left;
            SetImage(_imagePath);
        }
        public bool IsAttack { get; private set; }

        public Image Image { get; }

        public void SetMove()
        {
            Random random = new();
            int horizontalDirection = random.Next((int)MoveDirection.Left, (int)MoveDirection.Right + 1);
            int verticalDirection = random.Next((int)MoveDirection.Up, (int)MoveDirection.Down + 1);
            _moveDirection = (_zombieWalksHorizontally) ? horizontalDirection : verticalDirection;
            _zombieWalksHorizontally = !_zombieWalksHorizontally;
            TryMakeTurn(_moveDirection);
        }

        public void Move()
        {
            switch (_moveDirection)
            {
                case (int)MoveDirection.Left:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) - _speed);
                    break;
                case (int)MoveDirection.Right:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) + _speed);
                    break;
                case (int)MoveDirection.Up:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) - _speed);
                    break;
                case (int)MoveDirection.Down:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) + _speed);
                    break;
            }
        }

        public void TryMove(List<Rect> walls)
        {
            Rect zombieHitBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            foreach (var block in walls)
            {
                if (zombieHitBox.IntersectsWith(block))
                {
                    switch (_moveDirection)
                    {
                        case (int)MoveDirection.Left:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) + (_speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Right:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) - (_speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Up:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) + (_speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Down:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) - (_speed + DistanceToWall));
                            break;
                    }

                    SetMove();
                }
            }
        }

        public void TryMoveWithHunt(List<Rect> walls)
        {
            Rect zombieHitBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            foreach (var block in walls)
            {
                if (zombieHitBox.IntersectsWith(block))
                {
                    switch (_moveDirection)
                    {
                        case (int)MoveDirection.Left:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) + (_speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Right:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) - (_speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Up:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) + (_speed + DistanceToWall));
                            break;
                        case (int)MoveDirection.Down:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) - (_speed + DistanceToWall));
                            break;
                    }

                    MoveWithHuntAlongWall();
                }
            }
        }

        public void Step()
        {
            SetImage(_imagesWalk[_stepCounter]);
            _stepCounter++;
            _stepCounter = (_stepCounter == StepCount) ? 0 : _stepCounter;
        }

        public void Hunt(int direction)
        {
            if (direction != _moveDirection && direction != 0)
            {
                _lastSuccessfulDirection = _moveDirection;
                _moveDirection = direction;
                TryMakeTurn(_moveDirection);
            }
        }

        public bool Attack()
        {
            SetImage(_imagesAttack[_attackCounter]);
            _attackCounter++;
            return _attackCounter > AttackCount - 1;
        }

        public void Stop()
        {
            _moveDirection = (int)MoveDirection.None;
        }

        public bool TryCatchPlayer(Player player)
        {
            Rect zombieCatchBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            Rect playerCatchBox = new(Canvas.GetLeft(player.Image), Canvas.GetTop(player.Image), Player.Width, Player.Height);
            IsAttack = zombieCatchBox.IntersectsWith(playerCatchBox);
            return IsAttack;
        }

        public void Accelerate()
        {
            if (_speed < MaxSpeed)
            {
                _speed += _speed * 0.05;
            }
        }

        private void SetImage(string imagePath)
        {
            BitmapImage zombieImage = new(new Uri(imagePath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(zombieImage, new ScaleTransform(ImageScale, ImageScale));
            Image.Source = reducedBitmap;
            Image.Width = reducedBitmap.Width;
            Image.Height = reducedBitmap.Height;
        }

        private void TryMakeTurn(int zombieLook)
        {
            if (zombieLook == (int)LookDirection.Left && _zombieLook != (int)LookDirection.Left)
            {
                _zombieLook = (int)LookDirection.Left;
                Image.FlowDirection = FlowDirection.LeftToRight;
            }
            else if (zombieLook == (int)LookDirection.Right && _zombieLook != (int)LookDirection.Right)
            {
                _zombieLook = (int)LookDirection.Right;
                Image.FlowDirection = FlowDirection.RightToLeft;
            }
        }

        private void MoveWithHuntAlongWall()
        {
            int acceleration = 3;
            switch (_lastSuccessfulDirection)
            {
                case (int)MoveDirection.Left:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) - (_speed + acceleration));
                    break;
                case (int)MoveDirection.Right:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) + (_speed + acceleration));
                    break;
                case (int)MoveDirection.Up:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) - (_speed + acceleration));
                    break;
                case (int)MoveDirection.Down:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) + (_speed + acceleration));
                    break;
            }

        }


    }
}

