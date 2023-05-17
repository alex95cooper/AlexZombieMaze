using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlexMazeEngine
{
    public class MapBuilder
    {
        private const int MazeBlockSize = 50;
        private readonly Canvas _canvas;
        private readonly bool[,] _maze;

        public MapBuilder()
        {
            MazeGenerator generator = new();
            _maze = generator.GetMaze();
            _canvas = new();
            BuildMap();
        }

        public bool[,] Maze => _maze;
        public Canvas Canvas => _canvas;
        public List<Rect> Walls { get; } = new();

        public static void AddUiElementToCanvas(Canvas canvas, UIElement uIElement, int left, int top)
        {
            Canvas.SetLeft(uIElement, left);
            Canvas.SetTop(uIElement, top);
            canvas.Children.Add(uIElement);
        }

        public void BuildMap()
        {
            int blockTop = 0;
            for (int column = 0; column < _maze.GetLength(0); column++)
            {
                int blockLeft = 0;
                for (int row = 0; row < _maze.GetLength(1); row++)
                {
                    CreateMazeItem(column, row, blockLeft, blockTop);
                    blockLeft += MazeBlockSize;
                }

                blockTop += MazeBlockSize;
            }
        }

        private void CreateMazeItem(int column, int row, int blockeft, int blockTop)
        {
            Rectangle rect = new();
            rect.Width = rect.Height = MazeBlockSize;
            BitmapImage mazeImage = (_maze[column, row]) ?
                new(new Uri(@"Images\Ground.png", UriKind.Relative)) :
                new(new Uri(@"Images\Wall.png", UriKind.Relative));
            TryAddWall(column, row);
            ImageBrush myImageBrush = new(mazeImage);
            rect.Fill = myImageBrush;
            AddUiElementToCanvas(_canvas, rect, blockeft, blockTop);
        }

        private void TryAddWall(int column, int row)
        {
            if (!_maze[column, row])
            {
                Walls.Add(new Rect(row * MazeBlockSize, 
                    column * MazeBlockSize, MazeBlockSize, MazeBlockSize));
            }
        }
    }
}
