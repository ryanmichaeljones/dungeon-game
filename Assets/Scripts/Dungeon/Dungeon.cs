using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Assets.Scripts
{
    public class Dungeon : MonoBehaviour
    {
        [SerializeField] private Room _roomPrefab;
        [SerializeField] private Tunnel _tunnelPrefab;
        [SerializeField] private Transform _roomsParent;
        [SerializeField] private Transform _tunnelsParent;

        private RoomPlacer _roomPlacer;
        private TunnelPlacer _tunnelPlacer;

        public void Awake()
        {
            _roomPlacer = new(_roomPrefab, _roomsParent);
            _tunnelPlacer = new(_tunnelPrefab, _tunnelsParent);
        }

        public void Initialize(int roomCount, int dungeonRadius)
        {
            ReadOnlyCollection<Coordinate> roomPositions = _roomPlacer.PlaceRooms(roomCount, dungeonRadius);
            List<(Coordinate From, Coordinate To)> solutions = MSTSolver.Solve(roomPositions);

            foreach ((Coordinate From, Coordinate To) in solutions)
            {
                List<Coordinate> path = AStarPathfinder.Solve(From, To, dungeonRadius);
                _tunnelPlacer.PlaceTunnels(path, _roomPlacer.IsInsideAnyRoom);
            }
        }
    }
}
