using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using AlexMazeEngine.Humanoids;

namespace AlexMazeEngine.Generators
{
    public class PathGenerator
    {
        private const int MazeBlockSize = 50;
        private const int MazeSize = 11;
        private const int FreeWay = -1;
        private const int Wall = -2;

        public static MoveDirection GetDirection(bool[,] maze, Zombie zombie, Player player)
        {
            System.Drawing.Point start = ToPoint(zombie);
            System.Drawing.Point target = ToPoint(player);
            int[,] intMaze = ToIntArray(maze);
            intMaze = DrawPath(intMaze, start, target);
            return (CheckIfTargetBeside(intMaze, start, target, zombie, player, out MoveDirection direction)) ? 
                direction : GenerateDirection(intMaze, start);
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

        private static bool CheckIfTargetBeside(int[,] maze, System.Drawing.Point start, System.Drawing.Point target, Zombie zombie, Player player, out MoveDirection direction)
        {
            direction = MoveDirection.None;
            if (maze[start.Y, start.X] == maze[target.Y, target.X])
            {
                if ((int)Canvas.GetTop(zombie.Image) == (int)Canvas.GetTop(player.Image))
                {
                    direction = (Canvas.GetLeft(zombie.Image) > Canvas.GetLeft(player.Image)) ? MoveDirection.Left : MoveDirection.Right;
                }

                return true;
            }

            return false;
        }

        private static MoveDirection GenerateDirection(int[,] maze, System.Drawing.Point start)
        {
            MoveDirection direction = 0;
            int rightWay = maze[start.Y, start.X] - 1;
            if (maze[start.Y, start.X - 1] == rightWay)
            {
                direction = MoveDirection.Left;
            }
            else if (maze[start.Y, start.X + 1] == rightWay)
            {
                direction = MoveDirection.Right;
            }
            else if (maze[start.Y - 1, start.X] == rightWay)
            {
                direction = MoveDirection.Up;
            }
            else if (maze[start.Y + 1, start.X] == rightWay)
            {
                direction = MoveDirection.Down;
            }

            return direction;
        }

        private static System.Drawing.Point ToPoint(Zombie zombie)
        {
            int column = (zombie.MoveDirection == MoveDirection.Up) ?
               (int)((Canvas.GetTop(zombie.Image) + Humanoid.Height) / MazeBlockSize) : (int)(Canvas.GetTop(zombie.Image) / MazeBlockSize);
            int row = (zombie.MoveDirection == MoveDirection.Left) ?
                (int)((Canvas.GetLeft(zombie.Image) + zombie.Width) / MazeBlockSize) : (int)(Canvas.GetLeft(zombie.Image) / MazeBlockSize);
            return new(row, column);
        }

        private static System.Drawing.Point ToPoint(Player player)
        {
            int column = (int)(Canvas.GetTop(player.Image) / MazeBlockSize);
            int row = (int)(Canvas.GetLeft(player.Image) / MazeBlockSize);
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
