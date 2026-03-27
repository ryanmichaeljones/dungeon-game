using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class RoomPlacer
    {
        private readonly List<Rect> _roomBounds = new();
        private readonly Room _roomPrefab;
        private readonly Transform _roomsParent;

        private const int RoomOverlapThreshold = 2; // consider the dungeon room size from coordinate
        private const float MinRoomWidth = 1.5f;
        private const float MaxRoomWidth = 2.5f;
        private const float MinRoomHeight = 1.5f;
        private const float MaxRoomHeight = 2.5f;
        private const float RoomScaleY = 0.2f;
        private const float RoomPositionY = 0.0f;

        public RoomPlacer(Room roomPrefab, Transform roomsParent)
        {
            _roomPrefab = roomPrefab;
            _roomsParent = roomsParent;
        }

        public ReadOnlyCollection<Coordinate> PlaceRooms(int roomCount, int dungeonRadius)
        {
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
            Room newRoom = Object.Instantiate(_roomPrefab);
            float width = Random.Range(MinRoomWidth, MaxRoomWidth);
            float height = Random.Range(MinRoomHeight, MaxRoomHeight);
            float scaleX = newRoom.transform.localScale.x * width;
            float scaleZ = newRoom.transform.localScale.z * height;

            newRoom.name = $"{x}, {z}";
            newRoom.transform.parent = _roomsParent;
            newRoom.transform.localScale = new Vector3(scaleX, RoomScaleY, scaleZ);
            newRoom.transform.localPosition = new Vector3(x, RoomPositionY, z);

            _roomBounds.Add(new Rect(x - scaleX / 2f, z - scaleZ / 2f, scaleX, scaleZ));
        }

        public bool IsInsideAnyRoom(float worldX, float worldZ)
        {
            foreach (Rect bounds in _roomBounds)
            {
                if (bounds.Contains(new Vector2(worldX, worldZ)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
