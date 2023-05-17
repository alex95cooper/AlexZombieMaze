using AlexMazeEngine.Generators;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AlexMazeEngine
{
    public static class StartPositionGenerator
    {
        public static Point GetPlayerPosition(bool[,] maze)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j])
                    {
                        return new(i, j);
                    }
                }
            }

            return new(0, 0);
        }

        public static Point GetFirstZombiePosition(bool[,] maze)
        {
            for (int column = maze.GetLength(0) - 1; column > -1; column--)
            {
                for (int row = maze.GetLength(1) - 1; 0 <= row; row--)
                {
                    if (maze[column, row])
                    {
                        return new(row, column);
                    }
                }
            }

            return new(0, 0);
        }

        public static Point GetSecondZombiePosition(bool[,] maze)
        {
            for (int column = 0; column < maze.GetLength(0); column++)
            {
                for (int row = maze.GetLength(1) - 1; 0 <= row; row--)
                {
                    if (maze[column, row])
                    {
                        return new(row, column);
                    }
                }
            }

            return new(0, 0);
        }

        public static List<Point> GetCoinsPositions(bool[,] maze, int coinsQuantity)
        {
            List<Point> coinPositions = new();
            for (int index = 0; index < coinsQuantity; index++)
            {
                Point point = MazePointRandomizer.GetRandomPoint(0, maze.GetLength(0) - 1, 0, maze.GetLength(1) - 1, coinsQuantity, index);
                if (CheckIfPointRepeat(point, coinPositions) || maze[point.Y, point.X] == false)
                {
                    index--;
                }
                else
                {
                    coinPositions.Add(point);
                }
            }

            return coinPositions;
        }

        public static Point GetNewCoinPosition(bool[,] maze, List<Coin> coins)
        {
            List<Point> coinPositions = new();
            foreach (Coin coin in coins)
            {
                coinPositions.Add(coin.Position);
            }

            Point point = new();
            for (int index = 0; index < 1; index++)
            {
                Random random = new();
                point = new(random.Next(1, maze.GetLength(0)), random.Next(1, maze.GetLength(1)));
                if (CheckIfPointRepeat(point, coinPositions) || maze[point.Y, point.X] == false)
                {
                    index--;
                }
            }

            return point;
        }

        private static bool CheckIfPointRepeat(Point point, List<Point> coinPositions)
        {
            foreach (var coinPosition in coinPositions)
            {
                if (point.X == coinPosition.X && point.Y == coinPosition.Y)
                {
                    return true;
                }
            }

            return false;
        }
    }
}