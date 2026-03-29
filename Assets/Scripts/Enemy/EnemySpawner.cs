using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class EnemySpawner
    {
        private readonly Enemy _meleeEnemyPrefab;
        private readonly Enemy _rangedEnemyPrefab;
        private readonly Transform _enemyParent;

        public EnemySpawner(Enemy meleeEnemyPrefab, Enemy rangedEnemyPrefab, Transform enemyParent)
        {
            _meleeEnemyPrefab = meleeEnemyPrefab;
            _rangedEnemyPrefab = rangedEnemyPrefab;
            _enemyParent = enemyParent;
        }

        public void SpawnEnemies(DungeonGrid grid, int skipRoomIndex)
        {
            for (int i = 0; i < grid.RoomCount; i++)
            {
                if (i == skipRoomIndex) continue;

                RoomData room = grid.GetRoom(i);
                List<Coordinate> interiorCells = GetInteriorCells(room, grid);
                if (interiorCells.Count == 0) continue;

                int maxEnemies = Mathf.Clamp(interiorCells.Count / 4, 1, 3);
                int enemyCount = Random.Range(1, maxEnemies + 1);

                for (int j = 0; j < enemyCount; j++)
                {
                    Coordinate cell = interiorCells[Random.Range(0, interiorCells.Count)];
                    bool isRanged = Random.value < 0.3f;
                    Enemy prefab = isRanged ? _rangedEnemyPrefab : _meleeEnemyPrefab;

                    Enemy enemy = Object.Instantiate(prefab, new Vector3(cell.X, 0.5f, cell.Z), Quaternion.identity);
                    enemy.transform.parent = _enemyParent;
                }
            }
        }

        private static List<Coordinate> GetInteriorCells(RoomData room, DungeonGrid grid)
        {
            List<Coordinate> interior = new();
            foreach (Coordinate cell in room.Cells)
            {
                if (grid.IsFloor(cell.X + 1, cell.Z) &&
                    grid.IsFloor(cell.X - 1, cell.Z) &&
                    grid.IsFloor(cell.X, cell.Z + 1) &&
                    grid.IsFloor(cell.X, cell.Z - 1))
                {
                    interior.Add(cell);
                }
            }
            return interior;
        }
    }
}
