using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexMazeEngine
{
    public class PositionGenerator
    {
        private readonly bool[,] _maze;

        PositionGenerator(bool[,] maze)
        {
            _maze = maze;
        }

        public static int CoinsQuantity => 10;

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
                Point point = GetRandomPoint(maze, coinsQuantity, index);
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

        private static Point GetRandomPoint(bool[,] maze, int coinsQuantity, int coinIndex)
        {
            Random random = new();
            int halfColunmSize = maze.GetLength(0) / 2;
            int halfRowSize = maze.GetLength(1) / 2;
            int column, row;

            if (coinIndex < (int)(coinsQuantity * 0.25))
            {
                column = random.Next(1, halfColunmSize);
                row = random.Next(1, halfRowSize);
            }
            else if (coinIndex < (int)(coinsQuantity * 0.5))
            {
                column = random.Next(1, halfColunmSize);
                row = random.Next(halfRowSize, maze.GetLength(0) - 2);
            }
            else if (coinIndex < (int)(coinsQuantity * 0.75))
            {
                column = random.Next(halfColunmSize, maze.GetLength(0) - 2);
                row = random.Next(1, halfRowSize);
            }
            else
            {
                column = random.Next(halfColunmSize, maze.GetLength(0) - 2);
                row = random.Next(halfRowSize, maze.GetLength(0));
            }

            return new(row, column);
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
