using System.Drawing;
using System.Collections.Generic;
using AlexMazeEngine.Generators;

namespace AlexMazeEngine
{
    public class MazeGenerator
    {
        private const int PillarQuantity = 8;
        private const int SmallMazeSize = 9;
        private const int LargeMazeSize = 11;

        private readonly List<Point> _pillars = new();

        private bool[,] _maze = new bool[LargeMazeSize, LargeMazeSize];

        public MazeGenerator()
        {
            GenerateNewMaze();
        }

        public bool[,] GetMaze()
        {
            return _maze;
        }

        private void GenerateNewMaze()
        {
            FillMazeWithGround();
            FillMazeWithPillars();
            FillMazeWithWalls();
            AddExternalWalls();
        }

        private void FillMazeWithGround()
        {
            for (int column = 0; column < _maze.GetLength(0); column++)
            {
                for (int row = 0; row < _maze.GetLength(1); row++)
                {
                    _maze[column, row] = true;
                }
            }
        }

        private void FillMazeWithPillars()
        {
            for (int index = 0; index < PillarQuantity; index++)
            {
                Point point = MazePointRandomizer.GetRandomPoint(1, LargeMazeSize - 2, 1, LargeMazeSize - 2, PillarQuantity, index);
                if (CheckIfPointInTheCorner(point) || CheckPillarsBeside(point))
                {
                    index--;
                }
                else
                {
                    _maze[point.Y, point.X] = false;
                    _pillars.Add(point);
                }
            }
        }

        private static bool CheckIfPointInTheCorner(Point point)
        {
            if (point.Y is 1 or (LargeMazeSize - 2))
            {
                return point.X is 1 or (LargeMazeSize - 2);
            }

            return false;
        }

        private bool CheckPillarsBeside(Point point)
        {
            for (int column = point.Y - 1; column <= point.Y + 1; column++)
            {
                for (int row = point.X - 1; row <= point.X + 1; row++)
                {
                    if (_maze[column, row] == false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void FillMazeWithWalls()
        {
            WallGenerator builder = new(_maze, _pillars);
            _maze = builder.BuildWalls();
        }

        private void AddExternalWalls()
        {
            ReduceMaze();
            bool[,] largeMaze = new bool[LargeMazeSize, LargeMazeSize];
            for (int column = 0; column < SmallMazeSize; column++)
            {
                for (int row = 0; row < SmallMazeSize; row++)
                {
                    largeMaze[column + 1, row + 1] = _maze[column, row];
                }
            }

            _maze = largeMaze;
        }

        private void ReduceMaze()
        {
            bool[,] smallMaze = new bool[SmallMazeSize, SmallMazeSize];
            for (int column = 0; column < SmallMazeSize; column++)
            {
                for (int row = 0; row < SmallMazeSize; row++)
                {
                    smallMaze[column, row] = _maze[column + 1, row + 1];
                }
            }

            _maze = smallMaze;
        }
    }
}