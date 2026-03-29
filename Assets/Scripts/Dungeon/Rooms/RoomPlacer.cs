using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class RoomPlacer
    {
        private DungeonGrid _grid;

        private const int RoomOverlapThreshold = 5;
        private const int MinRoomHalf = 1;  // half-extent: room will be 3 cells min (1+1+center)
        private const int MaxRoomHalf = 3;  // half-extent: room will be 7 cells max (3+1+3)

        public RoomPlacer() { }

        public ReadOnlyCollection<Coordinate> PlaceRooms(int roomCount, int dungeonRadius, DungeonGrid grid)
        {
            _grid = grid;

            List<Coordinate> validPositions = GetValidRoomPositions(dungeonRadius);
            List<Coordinate> roomPositions = new();

            for (int i = 0; i < roomCount; i++)
            {
                if (validPositions.Count == 0)
                {
                    Debug.LogWarning($"No valid positions remaining. Placed {roomPositions.Count}/{roomCount} rooms.");
                    break;
                }

                int index = Random.Range(0, validPositions.Count);
                Coordinate position = validPositions[index];
                roomPositions.Add(position);
                validPositions.RemoveAll(p => EvaluatePositionOverlap(p, position));

                CreateRoom(position.X, position.Z);
            }

            return roomPositions.AsReadOnly();
        }

        private static List<Coordinate> GetValidRoomPositions(int dungeonRadius)
        {
            List<Coordinate> validPositions = new();

            for (int x = 0; x < dungeonRadius; x++)
            {
                for (int z = 0; z < dungeonRadius; z++)
                {
                    Coordinate position = new(x, z);
                    validPositions.Add(position);
                }
            }

            return validPositions;
        }

        private static bool EvaluatePositionOverlap(Coordinate evaluatePosition, Coordinate newPosition)
        {
            int deltaX = Mathf.Abs(evaluatePosition.X - newPosition.X);
            int deltaZ = Mathf.Abs(evaluatePosition.Z - newPosition.Z);
            return deltaX <= RoomOverlapThreshold && deltaZ <= RoomOverlapThreshold;
        }

        private void CreateRoom(int x, int z)
        {
            int halfW = Random.Range(MinRoomHalf, MaxRoomHalf + 1);
            int halfH = Random.Range(MinRoomHalf, MaxRoomHalf + 1);
            _grid.MarkRect(x, z, halfW, halfH);
        }

        public bool IsInsideAnyRoom(float worldX, float worldZ) => _grid.IsFloor(Mathf.RoundToInt(worldX), Mathf.RoundToInt(worldZ));
    }
}
