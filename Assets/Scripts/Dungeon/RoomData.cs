using System.Collections.Generic;

namespace Assets.Scripts
{
    public readonly struct RoomData
    {
        public readonly Coordinate Center;
        public readonly int Width;
        public readonly int Height;
        public readonly List<Coordinate> Cells;

        public RoomData(Coordinate center, int width, int height, List<Coordinate> cells)
        {
            Center = center;
            Width = width;
            Height = height;
            Cells = cells;
        }
    }
}
