using System.Windows;
using System.Windows.Controls;

namespace AlexMazeEngine.Generators
{
    public class PathGenerator
    {
        private const int MazeBlockSize = 50;
        private const int MazeSize = 11;
        private const int FreeWay = -1;
        private const int Wall = -2;

        public static int GetDirection(bool[,] maze, Zombie zombie, Player player)
        {
            System.Drawing.Point start = ToPoint(zombie.Image);
            System.Drawing.Point target = ToPoint(player.Image);
            int[,] intMaze = ToIntArray(maze);
            intMaze = DrawPath(intMaze, start, target);
            return GenerateDirection(intMaze, start, target);
        }

        public static int[,] DrawPath(int[,] maze, System.Drawing.Point start, System.Drawing.Point target)
        {
            int step = 0;
            bool pathCompleted = false;
            maze[target.Y, target.X] = 0;
            while (!pathCompleted)
            {
                for (int column = 0; column < maze.GetLength(0); column++)
                {
                    for (int row = 0; row < maze.GetLength(1); row++)
                    {
                        if (maze[column, row] == step)
                        {
                            maze[column - 1, row] = (maze[column - 1, row] == FreeWay) ? step + 1 : maze[column - 1, row];
                            maze[column, row - 1] = (maze[column, row - 1] == FreeWay) ? step + 1 : maze[column, row - 1];
                            maze[column + 1, row] = (maze[column + 1, row] == FreeWay) ? step + 1 : maze[column + 1, row];
                            maze[column, row + 1] = (maze[column, row + 1] == FreeWay) ? step + 1 : maze[column, row + 1];
                        }
                    }
                }

                step++;
                if (maze[start.Y, start.X] == 0 || maze[start.Y, start.X] == step)
                {
                    pathCompleted = true;
                }
            }

            return maze;
        }

        private static int GenerateDirection(int[,] maze, System.Drawing.Point start, System.Drawing.Point target)
        {
            int direction = 0;
            int rightWay = maze[start.Y, start.X] - 1;
            if (maze[start.Y, start.X - 1] == rightWay)
            {
                direction = (int)MoveDirection.Left;
            }
            else if (maze[start.Y, start.X + 1] == rightWay)
            {
                direction = (int)MoveDirection.Right;
            }
            else if (maze[start.Y - 1, start.X] == rightWay)
            {
                direction = (int)MoveDirection.Up;
            }
            else if (maze[start.Y + 1, start.X] == rightWay)
            {
                direction = (int)MoveDirection.Down;
            }
            else if (maze[start.Y, start.X] == maze[target.Y, target.X])
            {
                direction = (int)MoveDirection.None;
            }

            return direction;
        }

        private static System.Drawing.Point ToPoint(UIElement uIElement)
        {
            int column = (int)(Canvas.GetTop(uIElement) / MazeBlockSize);
            int row = (int)(Canvas.GetLeft(uIElement) / MazeBlockSize);
            return new(row, column);
        }

        private static int[,] ToIntArray(bool[,] boolArray)
        {
            int[,] intArray = new int[boolArray.GetLength(0), boolArray.GetLength(1)];
            for (int column = 0; column < MazeSize; column++)
                for (int row = 0; row < MazeSize; row++)
                    intArray[column, row] = (boolArray[column, row]) ? FreeWay : Wall;

            return intArray;
        }
    }
}
