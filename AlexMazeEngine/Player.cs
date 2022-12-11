using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine
{
    public class Player
    {
        public const double ImageScale = 0.4;
        public const double Height = 25;
        public const double Width = 9;
        public const int Speed = 1;

        private bool _isStep;
        private int _moveDirection;
        private int _playersLook;
        private int _step;



        public Player(string imagepath)
        {
            BitmapImage playerImage = new(new Uri(imagepath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(playerImage, new ScaleTransform(ImageScale, ImageScale));
            CroppedBitmap frame = new(reducedBitmap, new Int32Rect(0, 0, (int)Width, (int)Height));
            Image = new Image();
            Image.Source = frame;
            Image.Width = frame.Width;
            Image.Height = frame.Height;
        }

        public int Step { get; set; }

        public Image Image { get; set; }

        public void SetMove(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _moveDirection = (int)MoveDirection.Left;
                    _playersLook = (int)PlayersLook.Left;
                    break;
                case Key.Right:
                    _moveDirection = (int)MoveDirection.Right;
                    _playersLook = (int)PlayersLook.Right;
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
            Rect playerHitBox = new(Canvas.GetLeft(Image), Canvas.GetTop(Image), Player.Width, Player.Height);
            foreach (var block in walls)
            {
                if (playerHitBox.IntersectsWith(block))
                {
                    switch (_moveDirection)
                    {
                        case (int)MoveDirection.Left:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) + 2);
                            break;
                        case (int)MoveDirection.Right:
                            Canvas.SetLeft(Image, Canvas.GetLeft(Image) - 2);
                            break;
                        case (int)MoveDirection.Up:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) + 2);
                            break;
                        case (int)MoveDirection.Down:
                            Canvas.SetTop(Image, Canvas.GetTop(Image) - 2);
                            break;
                    }

                    _moveDirection = (int)MoveDirection.None;
                }
            }
        }

        public void Stop()
        {
            _moveDirection = (int)MoveDirection.None;
            _isStep = false;
        }

        public void Steps()
        {
            _step = _step + 1;


        }


    }
}
