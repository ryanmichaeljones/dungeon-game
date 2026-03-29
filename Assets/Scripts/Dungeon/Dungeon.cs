using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Assets.Scripts
{
    public class Dungeon : MonoBehaviour
    {
        [SerializeField] private Transform _floorParent;
        [SerializeField] private Transform _wallParent;
        [SerializeField] private Material _floorMaterial;
        [SerializeField] private Material _wallMaterial;

        private RoomPlacer _roomPlacer;
        private TunnelPlacer _tunnelPlacer;
        private DungeonRenderer _renderer;
        private DungeonGrid _grid;

        public void Awake()
        {
            _roomPlacer = new();
            _tunnelPlacer = new();
            _renderer = new DungeonRenderer(_floorParent, _wallParent, _floorMaterial, _wallMaterial);
        }

        public void Initialize(int roomCount, int dungeonRadius)
        {
            _grid = new DungeonGrid();

            ReadOnlyCollection<Coordinate> roomPositions = _roomPlacer.PlaceRooms(roomCount, dungeonRadius, _grid);
            List<(Coordinate From, Coordinate To)> paths = MSTSolver.Solve(roomPositions);

            foreach ((Coordinate From, Coordinate To) in paths)
            {
                List<Coordinate> path = AStarPathfinder.Solve(From, To, dungeonRadius);
                _tunnelPlacer.PlaceTunnels(path, _grid);
            }

            _renderer.Render(_grid);
        }

        public Coordinate GetStartRoomCenter() => _grid.GetRoom(0).Center;
    }
}
