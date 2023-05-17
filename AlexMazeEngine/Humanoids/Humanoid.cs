using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine.Humanoids
{
    public class Humanoid
    {
        protected const double ImageScale = 0.5;

        protected double _speed;
        protected int _stepCounter;
        protected string _imagePath;
        protected MoveDirection _moveDirection;
        protected LookDirection _lookDirection;

        public Humanoid(string imagepath, double width, double speed)
        {
            Width = width;
            Image = new Image();
            _speed = speed;
            _imagePath = imagepath;
            SetImage(_imagePath);
        }

        public static int DistanceToWall => 1;
        public static double Height => 32;

        public double Width { get; }
        public Image Image { get; }

        public MoveDirection MoveDirection => _moveDirection;
        public double Speed => _speed;

        public void SetImage(string imagePath, int rectX = 0)
        {
            BitmapImage humanoidImage = new(new Uri(imagePath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(humanoidImage, new ScaleTransform(ImageScale, ImageScale));
            CroppedBitmap frame = new(reducedBitmap, new Int32Rect(rectX, 0, (int)Width, (int)Height));
            Image.Source = frame;
            Image.Width = frame.Width;
            Image.Height = frame.Height;
        }

        public virtual void SetMove() { }

        public void Move()
        {
            switch (MoveDirection)
            {
                case MoveDirection.Left:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) - Speed);
                    break;
                case MoveDirection.Right:
                    Canvas.SetLeft(Image, Canvas.GetLeft(Image) + Speed);
                    break;
                case MoveDirection.Up:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) - Speed);
                    break;
                case MoveDirection.Down:
                    Canvas.SetTop(Image, Canvas.GetTop(Image) + Speed);
                    break;
            }
        }

        public void TryMove(List<Rect> walls)
        {
            Rect hitBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Width, Height);
            foreach (var block in walls)
            {
                if (hitBox.IntersectsWith(block))
                {
                    switch (MoveDirection)
                    {
                        case MoveDirection.Left:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) + (Speed + DistanceToWall));
                            break;
                        case MoveDirection.Right:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) - (Speed + DistanceToWall));
                            break;
                        case MoveDirection.Up:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) + (Speed + DistanceToWall));
                            break;
                        case MoveDirection.Down:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) - (Speed + DistanceToWall));
                            break;
                    }

                    SetMoveOrImmobilize();
                }
            }
        }

        protected void SetMoveOrImmobilize()
        {
            if (this is Player)
            {
                _moveDirection = MoveDirection.None;
            }
            else
            {
                SetMove();
            }
        }
    }
}
