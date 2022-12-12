using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine
{
    public class Zombie
    {
        public const double ImageScale = 0.333;
        public const double Height = 21;
        public const double Width = 8;
        public const double MinSpeed = 0.5;
        public const double MaxSpeed = 10;
        public const int StepCount = 7;
        public const int StopDistance = 1;

        List<string> _imagesWalk;
        List<string> _imagesAttack;
        private string _imagePath;
        private double _speed;
        private int _zombieLook;
        private int _moveDirection;
        private int _stepCounter;
        
        public Zombie(List<string> imagesWalk, List<string> imagesAttack)
        {
            _imagesWalk = imagesWalk;
            _imagesAttack = imagesAttack;
            _imagePath = imagesWalk[0];
            Image = new Image();
            _speed = 0.5;
            _zombieLook = (int)LookDirection.Right;
            SetImage(_imagePath);
        }

        public Image Image { get; internal set; }

        private void SetImage(string imagePath)
        {
            BitmapImage zombieImage = new(new Uri(imagePath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(zombieImage, new ScaleTransform(ImageScale, ImageScale));
            Image.Source = reducedBitmap;
            Image.Width = reducedBitmap.Width;
            Image.Height = reducedBitmap.Height;
        }

        public void SetMove()
        {
            Random random = new();
            _moveDirection = random.Next(1,4);
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
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) + (_speed+StopDistance));
                            break;
                        case (int)MoveDirection.Right:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) - (_speed + StopDistance));
                            break;
                        case (int)MoveDirection.Up:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) + (_speed + StopDistance));
                            break;
                        case (int)MoveDirection.Down:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) - (_speed + StopDistance));
                            break;
                    }

                    SetMove();
                }
            }
        }

        public void Stop()
        {
            _moveDirection = (int)MoveDirection.None;
        }

        public void Step()
        {           
            SetImage(_imagesWalk[_stepCounter]);
            _stepCounter++;
            _stepCounter = (_stepCounter == StepCount) ? 0 : _stepCounter;
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

        public void Accelerate()
        {
            if (_speed < MaxSpeed)
            {
                _speed++;
            }           
        }
    }
}

