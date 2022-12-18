using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AlexMazeEngine
{
    public interface IHumanoid
    {
        public const double ImageScale = 0.5;
        public const int DistanceToWall = 1;

        public Image Image { get; }

        private static void SetImage(int rectX) { }

        public void Move() { }

        public void TryMove(List<Rect> walls) { }

        public void Stop() { }

        public void Step() { }

        private static void TryMakeTurn(int playerLook) { }

    }
}
