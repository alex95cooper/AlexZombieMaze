using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AlexMazeEngine
{
    public interface IHumanoid
    {
        public const double ImageScale = 0.5;
        public const int DistanceToWall = 1;

        public Image Image { get; }

        public void Move() { }

        public void TryMove(List<Rect> walls) { }

        public void Stop() { }

        public void Step() { }
    }
}
