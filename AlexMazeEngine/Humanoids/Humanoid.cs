using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine.Humanoids
{
    public abstract class Humanoid
    {
        protected const double ImageScale = 0.5;

        public const int DistanceToWall = 1;
        public const double Height = 32;

        protected int StepCounter;
        protected string ImagePath;
        protected LookDirection LookDirection;

        public Humanoid(string imagepath, double width, double speed)
        {
            Width = width;
            Image = new Image();
            Speed = speed;
            ImagePath = imagepath;
            SetImage(ImagePath);
        }


        public double Width { get; }
        public Image Image { get; }

        public MoveDirection MoveDirection { get; internal set; }
        public double Speed { get; internal set; }

        public void SetImage(string imagePath, int rectX = 0)
        {
            BitmapImage humanoidImage = new(new Uri(imagePath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(humanoidImage, new ScaleTransform(ImageScale, ImageScale));
            CroppedBitmap frame = new(reducedBitmap, new Int32Rect(rectX, 0, (int)Width, (int)Height));
            Image.Source = frame;
            Image.Width = frame.Width;
            Image.Height = frame.Height;
        }

        public abstract void AvoidTouchingWall();

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

                    AvoidTouchingWall();
                }
            }
        }
    }
}
