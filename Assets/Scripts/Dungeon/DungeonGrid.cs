using System.Collections.Generic;

namespace Assets.Scripts
{
    public class DungeonGrid
    {
        private readonly HashSet<Coordinate> _walkableFloorCells = new();
        private readonly Dictionary<int, RoomData> _roomDataByIndex = new();

        public int RoomCount => _roomDataByIndex.Count;

        public void MarkCell(int x, int z)
        {
            _walkableFloorCells.Add(new Coordinate(x, z));
        }

        public void MarkRect(int centerX, int centerZ, int halfW, int halfH)
        {
            List<Coordinate> cells = new();

            for (int x = centerX - halfW; x <= centerX + halfW; x++)
            {
                for (int z = centerZ - halfH; z <= centerZ + halfH; z++)
                {
                    Coordinate cell = new(x, z);
                    _walkableFloorCells.Add(cell);
                    cells.Add(cell);
                }
            }

            int roomIndex = _roomDataByIndex.Count;
            _roomDataByIndex[roomIndex] = new RoomData(
                new Coordinate(centerX, centerZ),
                halfW * 2 + 1,
                halfH * 2 + 1,
                cells
            );
        }

        public bool IsFloor(int x, int z) => _walkableFloorCells.Contains(new Coordinate(x, z));

        public bool IsWallEdge(int x, int z, Direction direction)
        {
            if (!IsFloor(x, z)) return false;

            return direction switch
            {
                Direction.North => !IsFloor(x, z + 1),
                Direction.South => !IsFloor(x, z - 1),
                Direction.East => !IsFloor(x + 1, z),
                Direction.West => !IsFloor(x - 1, z),
                _ => false
            };
        }

        public IReadOnlyCollection<Coordinate> GetAllFloorCells() => _walkableFloorCells;

        public RoomData GetRoom(int index) => _roomDataByIndex[index];
    }
}
