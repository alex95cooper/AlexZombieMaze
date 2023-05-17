using System.Collections.Generic;
using System.Drawing;

namespace AlexMazeEngine
{
    public class WallGenerator
    {
        private const int SmallWallSize = 1;
        private const int LargeWallSize = 2;
        private const int LargeMazeSize = 11;

        private readonly List<Point> _pillars;
        private readonly bool[,] _maze;

        public WallGenerator(bool[,] maze, List<Point> pillars)
        {
            _maze = maze;
            _pillars = pillars;
        }

        public bool[,] BuildWalls()
        {
            AddPlusShapedWalls(SmallWallSize);
            AddLargeDiagonalWalls(LargeWallSize);
            AddSmallDiagonalWalls(SmallWallSize);
            AddPlusShapedWalls(LargeWallSize);
            return _maze;
        }

        public void AddPlusShapedWalls(int plusSize)
        {
            foreach (var pillar in _pillars)
            {
                if (CheckIfPlusWallCanGrow(pillar, new(pillar.X, pillar.Y - plusSize)))
                    _maze[pillar.Y - plusSize, pillar.X] = false;

                if (CheckIfPlusWallCanGrow(pillar, new(pillar.X + plusSize, pillar.Y)))
                    _maze[pillar.Y, pillar.X + plusSize] = false;

                if (CheckIfPlusWallCanGrow(pillar, new(pillar.X - plusSize, pillar.Y)))
                    _maze[pillar.Y, pillar.X - plusSize] = false;

                if (CheckIfPlusWallCanGrow(pillar, new(pillar.X, pillar.Y + plusSize)))
                    _maze[pillar.Y + plusSize, pillar.X] = false;
            }
        }

        private bool CheckIfPlusWallCanGrow(Point pillar, Point growPoint)
        {
            if (pillar.Y > growPoint.Y && growPoint.Y > 1)
            {
                return CheckIfPassageIsFree(growPoint.Y - 1, growPoint.Y, growPoint.X - 1, growPoint.X + 1);
            }
            else if (pillar.X < growPoint.X && growPoint.X < _maze.GetLength(1) - 2)
            {
                return CheckIfPassageIsFree(growPoint.Y - 1, growPoint.Y + 1, growPoint.X, growPoint.X + 1);
            }
            else if (pillar.X > growPoint.X && growPoint.X > 1)
            {
                return CheckIfPassageIsFree(growPoint.Y - 1, growPoint.Y + 1, growPoint.X - 1, growPoint.X);
            }
            else if (pillar.Y < growPoint.Y && growPoint.Y < _maze.GetLength(0) - 2)
            {
                return CheckIfPassageIsFree(growPoint.Y, growPoint.Y + 1, growPoint.X - 1, growPoint.X + 1);
            }
            else
            {
                return false;
            }
        }

        private bool CheckIfPassageIsFree(int colMin, int colMax, int rowMin, int rowMax)
        {
            for (int column = colMin; column <= colMax; column++)
            {
                for (int row = rowMin; row <= rowMax; row++)
                {
                    if (_maze[column, row] == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void AddLargeDiagonalWalls(int diagonalSize)
        {
            foreach (var pillar in _pillars)
            {
                if (CheckIfWallCanGrowDiagonally(new(pillar.X - diagonalSize, pillar.Y - diagonalSize)))
                    _maze[pillar.Y - diagonalSize, pillar.X - diagonalSize] = false;

                if (CheckIfWallCanGrowDiagonally(new(pillar.X + diagonalSize, pillar.Y - diagonalSize)))
                    _maze[pillar.Y - diagonalSize, pillar.X + diagonalSize] = false;

                if (CheckIfWallCanGrowDiagonally(new(pillar.X + diagonalSize, pillar.Y + diagonalSize)))
                    _maze[pillar.Y + diagonalSize, pillar.X + diagonalSize] = false;

                if (CheckIfWallCanGrowDiagonally(new(pillar.X - diagonalSize, pillar.Y + diagonalSize)))
                    _maze[pillar.Y + diagonalSize, pillar.X - diagonalSize] = false;
            }
        }

        private bool CheckIfWallCanGrowDiagonally(Point diagonalPoint)
        {
            if (CheckIfPointNotOnBorder(diagonalPoint))
            {
                return _maze[diagonalPoint.Y - 1, diagonalPoint.X - 1] &&
                    _maze[diagonalPoint.Y - 1, diagonalPoint.X + 1] &&
                    _maze[diagonalPoint.Y + 1, diagonalPoint.X + 1] &&
                    _maze[diagonalPoint.Y + 1, diagonalPoint.X - 1];
            }

            return false;
        }

        private static bool CheckIfPointNotOnBorder(Point point)
        {
            return point.Y > 1 && point.X > 1 &&
                point.Y < LargeMazeSize - 1 &&
                point.X < LargeMazeSize - 1;
        }

        private void AddSmallDiagonalWalls(int diagonalSize)
        {
            foreach (var pillar in _pillars)
            {
                if (CheckIfWallCanGrowTopLeft(new(pillar.X - diagonalSize, pillar.Y - diagonalSize)))
                    _maze[pillar.Y - diagonalSize, pillar.X - diagonalSize] = false;

                if (CheckIfWallCanGrowTopRight(new(pillar.X + diagonalSize, pillar.Y - diagonalSize)))
                    _maze[pillar.Y - diagonalSize, pillar.X + diagonalSize] = false;

                if (CheckIfWallCanGrowDownRight(new(pillar.X + diagonalSize, pillar.Y + diagonalSize)))
                    _maze[pillar.Y + diagonalSize, pillar.X + diagonalSize] = false;

                if (CheckIfWallCanGrowDownLeft(new(pillar.X - diagonalSize, pillar.Y + diagonalSize)))
                    _maze[pillar.Y + diagonalSize, pillar.X - diagonalSize] = false;
            }
        }

        private bool CheckIfWallCanGrowTopLeft(Point diagonalPoint)
        {
            if (CheckIfPointNotOnBorder(diagonalPoint))
            {
                if (!_maze[diagonalPoint.Y, diagonalPoint.X + 1] && !_maze[diagonalPoint.Y + 1, diagonalPoint.X])
                {
                    return _maze[diagonalPoint.Y, diagonalPoint.X - 1] &&
                        _maze[diagonalPoint.Y - 1, diagonalPoint.X - 1] &&
                        _maze[diagonalPoint.Y - 1, diagonalPoint.X];
                }
            }

            return false;
        }

        private bool CheckIfWallCanGrowTopRight(Point diagonalPoint)
        {
            if (CheckIfPointNotOnBorder(diagonalPoint))
            {
                if (!_maze[diagonalPoint.Y + 1, diagonalPoint.X] && !_maze[diagonalPoint.Y, diagonalPoint.X - 1])
                {
                    return _maze[diagonalPoint.Y - 1, diagonalPoint.X] &&
                        _maze[diagonalPoint.Y - 1, diagonalPoint.X + 1] &&
                        _maze[diagonalPoint.Y, diagonalPoint.X + 1];
                }
            }

            return false;
        }

        private bool CheckIfWallCanGrowDownRight(Point diagonalPoint)
        {
            if (CheckIfPointNotOnBorder(diagonalPoint))
            {
                if (!_maze[diagonalPoint.Y, diagonalPoint.X - 1] && !_maze[diagonalPoint.Y - 1, diagonalPoint.X])
                {
                    return _maze[diagonalPoint.Y, diagonalPoint.X + 1] &&
                        _maze[diagonalPoint.Y + 1, diagonalPoint.X + 1] &&
                        _maze[diagonalPoint.Y + 1, diagonalPoint.X];
                }
            }

            return false;
        }

        private bool CheckIfWallCanGrowDownLeft(Point diagonalPoint)
        {
            if (CheckIfPointNotOnBorder(diagonalPoint))
            {
                if (!_maze[diagonalPoint.Y - 1, diagonalPoint.X] && !_maze[diagonalPoint.Y, diagonalPoint.X + 1])
                {
                    return _maze[diagonalPoint.Y + 1, diagonalPoint.X] &&
                        _maze[diagonalPoint.Y + 1, diagonalPoint.X - 1] &&
                        _maze[diagonalPoint.Y, diagonalPoint.X - 1];
                }
            }

            return false;
        }
    }
}
