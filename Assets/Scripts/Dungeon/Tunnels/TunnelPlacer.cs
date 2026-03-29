using System.Collections.Generic;

namespace Assets.Scripts
{
    public class TunnelPlacer
    {
        public void PlaceTunnels(List<Coordinate> path, DungeonGrid grid)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Coordinate from = path[i];
                Coordinate to = path[i + 1];

                int dx = to.X - from.X;
                int dz = to.Z - from.Z;

                // Mark the start cell and perpendicular neighbors for corridor width
                grid.MarkCell(from.X, from.Z);

                if (dx != 0 && dz == 0)
                {
                    // Horizontal movement — expand perpendicular (Z axis)
                    grid.MarkCell(from.X, from.Z - 1);
                    grid.MarkCell(from.X, from.Z + 1);
                }
                else if (dz != 0 && dx == 0)
                {
                    // Vertical movement — expand perpendicular (X axis)
                    grid.MarkCell(from.X - 1, from.Z);
                    grid.MarkCell(from.X + 1, from.Z);
                }
                else
                {
                    // Diagonal — expand both axes for wider junction
                    grid.MarkCell(from.X - 1, from.Z);
                    grid.MarkCell(from.X + 1, from.Z);
                    grid.MarkCell(from.X, from.Z - 1);
                    grid.MarkCell(from.X, from.Z + 1);
                }
            }

            // Mark the last cell in the path
            if (path.Count > 0)
            {
                Coordinate last = path[^1];
                grid.MarkCell(last.X, last.Z);
                grid.MarkCell(last.X - 1, last.Z);
                grid.MarkCell(last.X + 1, last.Z);
                grid.MarkCell(last.X, last.Z - 1);
                grid.MarkCell(last.X, last.Z + 1);
            }
        }
    }
}
