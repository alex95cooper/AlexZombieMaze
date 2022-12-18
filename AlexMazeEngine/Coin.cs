using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlexMazeEngine
{
    public class Coin
    {
        public const double ImageScale = 0.5;
        public const double Height = 15;
        public const double Width = 15;
        public const int RotateCount = 4;
        public const int DistanceToWall = 17;

        private readonly List<string> _imagesCoin;
        private readonly string _imagePath;
        private int _rotateCounter;

        public Coin(List<string> imagesCoin)
        {
            _imagesCoin = imagesCoin;
            _imagePath = imagesCoin[0];
            Image = new Image();
            SetImage(_imagePath);
        }

        public Image Image { get; internal set; }

        public Point Position { get; set; }

        private void SetImage(string imagePath)
        {
            BitmapImage zombieImage = new(new Uri(imagePath, UriKind.Relative));
            TransformedBitmap reducedBitmap = new(zombieImage, new ScaleTransform(ImageScale, ImageScale));
            Image.Source = reducedBitmap;
            Image.Width = reducedBitmap.Width;
            Image.Height = reducedBitmap.Height;
        }

        public void Rotate()
        {
            SetImage(_imagesCoin[_rotateCounter]);
            _rotateCounter++;
            _rotateCounter = (_rotateCounter == RotateCount) ? 0 : _rotateCounter;
        }
    }
}

