using System;
using System.Drawing;
using System.Collections.Generic;

namespace AlexMazeEngine
{
    public class MazeGenerator
    {
        private const int SmallWallSize = 1;
        private const int LargeWallSize = 2;
        private const int PillarQuantity = 8;
        private const int SmallMazeSize = 9;
        private const int LargeMazeSize = 11;

        private readonly List<Point> _pillars;

        private bool[,] _maze;

        public MazeGenerator()
        {
            _maze = new bool[SmallMazeSize, SmallMazeSize];
            _pillars = new();
        }

        public bool[,] GenerateNewMaze()
        {
            FillMazeWithGround();
            FillMazeWithPillars();
            FillMazeWithWalls();
            _maze = AddExternalWalls();
            return _maze;
        }

        public static Point GetRandomPoint(bool[,] maze, int elementQuantity, int elementIndex)
        {
            Random random = new();
            int halfColunmSize = maze.GetLength(0) / 2;
            int halfRowSize = maze.GetLength(1) / 2;
            int column, row;

            if (elementIndex < PillarQuantity * 0.25)
            {
                column = random.Next(halfColunmSize);
                row = random.Next(halfRowSize);
            }
            else if (elementIndex < PillarQuantity * 0.5)
            {
                column = random.Next(halfColunmSize);
                row = random.Next(halfRowSize, maze.GetLength(1) - 1);
            }
            else if (elementIndex < PillarQuantity * 0.75)
            {
                column = random.Next(halfColunmSize, maze.GetLength(0) - 1);
                row = random.Next(halfRowSize);
            }
            else
            {
                column = random.Next(halfColunmSize, maze.GetLength(0) - 1);
                row = random.Next(halfRowSize, maze.GetLength(1) - 1);
            }

            return new(row, column);
        }

        private void FillMazeWithGround()
        {
            for (int column = 0; column < SmallMazeSize; column++)
            {
                for (int row = 0; row < SmallMazeSize; row++)
                {
                    _maze[column, row] = true;
                }
            }
        }

        private void FillMazeWithPillars()
        {
            for (int index = 0; index < PillarQuantity; index++)
            {
                Point point = GetRandomPoint(_maze, PillarQuantity, index);
                if (CheckIfPointRepeat(point) || CheckIfPointInTheCorner(point) || CheckIfPointsContact(point))
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

        private bool CheckIfPointRepeat(Point point)
        {
            foreach (var pillar in _pillars)
            {
                if (point.Y == pillar.Y && point.X == pillar.X)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckIfPointInTheCorner(Point point)
        {
            if (point.Y is 0 or (SmallMazeSize - 1))
            {
                return point.X is 0 or (SmallMazeSize - 1);
            }

            return false;
        }

        private bool CheckIfPointsContact(Point point)
        {
            if (point.X == 0)
            {
                return CheckPillarsBeside(point.Y - 1, point.Y + 1, point.X, point.X + 1);
            }
            else if (point.Y == 0)
            {
                return CheckPillarsBeside(point.Y, point.Y + 1, point.X - 1, point.X + 1);
            }
            else if (point.X == SmallMazeSize - 1)
            {
                return CheckPillarsBeside(point.Y - 1, point.Y + 1, point.X - 1, point.X);
            }
            else if (point.Y == SmallMazeSize - 1)
            {
                return CheckPillarsBeside(point.Y - 1, point.Y, point.X - 1, point.X + 1);
            }
            else
            {
                return CheckPillarsBeside(point.Y - 1, point.Y + 1, point.X - 1, point.X + 1);
            }
        }

        private bool CheckPillarsBeside(int colMin, int colMax, int rowMin, int rowMax)
        {
            for (int column = colMin; column <= colMax; column++)
            {
                for (int row = rowMin; row <= rowMax; row++)
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
            AddPlusShapedWalls(SmallWallSize);
            AddLargeDiagonalWalls(LargeWallSize);
            AddSmallDiagonalWalls(SmallWallSize);
            AddPlusShapedWalls(LargeWallSize);
        }

        private void AddPlusShapedWalls(int plusSize)
        {
            foreach (var pillar in _pillars)
            {
                if (ChekIfWallCanGrowTop(new(pillar.X, pillar.Y - plusSize)))
                    _maze[pillar.Y - plusSize, pillar.X] = false;

                if (ChekIfWallCanGrowLeft(new(pillar.X + plusSize, pillar.Y)))
                    _maze[pillar.Y, pillar.X + plusSize] = false;

                if (ChekIfWallCanGrowRight(new(pillar.X - plusSize, pillar.Y)))
                    _maze[pillar.Y, pillar.X - plusSize] = false;

                if (ChekIfWallCanGrowDown(new(pillar.X, pillar.Y + plusSize)))
                    _maze[pillar.Y + plusSize, pillar.X] = false;
            }
        }

        private bool ChekIfWallCanGrowTop(Point topPoint)
        {
            if (topPoint.Y < 1)
            {
                return false;
            }
            else if (topPoint.X == 0)
            {
                return CheckIfPassageIsFree(topPoint.Y - 1, topPoint.Y, topPoint.X, topPoint.X + 1);
            }
            else if (topPoint.X == SmallMazeSize - 1)
            {
                return CheckIfPassageIsFree(topPoint.Y - 1, topPoint.Y, topPoint.X - 1, topPoint.X);
            }
            else
            {
                return CheckIfPassageIsFree(topPoint.Y - 1, topPoint.Y, topPoint.X - 1, topPoint.X + 1);
            }
        }

        private bool ChekIfWallCanGrowLeft(Point leftPoint)
        {
            if (leftPoint.X > SmallMazeSize - 2)
            {
                return false;
            }
            else if (leftPoint.Y == 0)
            {
                return CheckIfPassageIsFree(leftPoint.Y, leftPoint.Y + 1, leftPoint.X, leftPoint.X + 1);
            }
            else if (leftPoint.Y == SmallMazeSize - 1)
            {
                return CheckIfPassageIsFree(leftPoint.Y - 1, leftPoint.Y, leftPoint.X, leftPoint.X + 1);
            }
            else
            {
                return CheckIfPassageIsFree(leftPoint.Y - 1, leftPoint.Y + 1, leftPoint.X, leftPoint.X + 1);
            }
        }

        private bool ChekIfWallCanGrowRight(Point rightPoint)
        {
            if (rightPoint.X < 1)
            {
                return false;
            }
            else if (rightPoint.Y == 0)
            {
                return CheckIfPassageIsFree(rightPoint.Y, rightPoint.Y + 1, rightPoint.X - 1, rightPoint.X);
            }
            else if (rightPoint.Y == SmallMazeSize - 1)
            {
                return CheckIfPassageIsFree(rightPoint.Y - 1, rightPoint.Y, rightPoint.X - 1, rightPoint.X);
            }
            else
            {
                return CheckIfPassageIsFree(rightPoint.Y - 1, rightPoint.Y + 1, rightPoint.X - 1, rightPoint.X);
            }
        }

        private bool ChekIfWallCanGrowDown(Point downPoint)
        {
            if (downPoint.Y > SmallMazeSize - 2)
            {
                return false;
            }
            else if (downPoint.X == 0)
            {
                return CheckIfPassageIsFree(downPoint.Y, downPoint.Y + 1, downPoint.X, downPoint.X + 1);
            }
            else if (downPoint.X == SmallMazeSize - 1)
            {
                return CheckIfPassageIsFree(downPoint.Y, downPoint.Y + 1, downPoint.X - 1, downPoint.X);
            }
            else
            {
                return CheckIfPassageIsFree(downPoint.Y, downPoint.Y + 1, downPoint.X - 1, downPoint.X + 1);
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
            return point.Y > 0 && point.X > 0 &&
                point.Y < SmallMazeSize - 1 &&
                point.X < SmallMazeSize - 1;
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

        private bool[,] AddExternalWalls()
        {
            bool[,] _largeMaze = new bool[LargeMazeSize, LargeMazeSize];
            for (int column = 0; column < SmallMazeSize; column++)
            {
                for (int row = 0; row < SmallMazeSize; row++)
                {
                    _largeMaze[column + 1, row + 1] = _maze[column, row];
                }
            }

            return _largeMaze;
        }
    }
}