using System;
using System.Drawing;

namespace AlexMazeEngine.Generators
{
    internal static class MazePointRandomizer
    {
        public static Point GetRandomPoint(int minY, int maxY, int minX, int maxX, int elementQuantity, int elementIndex)
        {
            Random random = new();
            int midY = (maxY - minY) / 2;
            int midX = (maxX - minX) / 2;
            if (elementIndex < elementQuantity * 0.25)
            {
                return new(random.Next(minX, midX), random.Next(minY, midY));
            }
            else if (elementIndex < elementQuantity * 0.5)
            {
                return new(random.Next(midX, maxX), random.Next(minY, midY));
            }
            else if (elementIndex < elementQuantity * 0.75)
            {
                return new(random.Next(minX, midX), random.Next(midY, maxY));
            }
            else
            {
                return new(random.Next(midX, maxX), random.Next(midY, maxY));
            }
        }
    }
}
