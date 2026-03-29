using UnityEngine;

namespace Assets.Scripts
{
    public class DungeonRenderer
    {
        private readonly Transform _floorParent;
        private readonly Transform _wallParent;
        private readonly Material _floorMaterial;
        private readonly Material _wallMaterial;

        private const float WallHeight = 1f;
        private const float WallThickness = 0.1f;
        private const float FloorThickness = 0.1f;

        public DungeonRenderer(Transform floorParent, Transform wallParent, Material floorMaterial, Material wallMaterial)
        {
            _floorParent = floorParent;
            _wallParent = wallParent;
            _floorMaterial = floorMaterial != null ? floorMaterial : CreateDefaultMaterial(new Color(0.2f, 0.2f, 0.2f));
            _wallMaterial = wallMaterial != null ? wallMaterial : CreateDefaultMaterial(new Color(0.5f, 0.45f, 0.35f));
        }

        private static Material CreateDefaultMaterial(Color color)
        {
            Material mat = new(Shader.Find("Standard"));
            mat.color = color;
            return mat;
        }

        public void Render(DungeonGrid grid)
        {
            foreach (Coordinate cell in grid.GetAllFloorCells())
            {
                CreateFloorTile(cell.X, cell.Z);
                CreateWalls(cell.X, cell.Z, grid);
            }

            StaticBatchingUtility.Combine(_floorParent.gameObject);
            StaticBatchingUtility.Combine(_wallParent.gameObject);
        }

        private void CreateFloorTile(int x, int z)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = $"Floor {x},{z}";
            tile.transform.parent = _floorParent;
            tile.transform.localPosition = new Vector3(x, 0f, z);
            tile.transform.localScale = new Vector3(1f, FloorThickness, 1f);
            tile.GetComponent<Renderer>().sharedMaterial = _floorMaterial;
            tile.isStatic = true;
        }

        private void CreateWalls(int x, int z, DungeonGrid grid)
        {
            if (grid.IsWallEdge(x, z, Direction.North))
                CreateWall(x, z + 0.5f, 1f, WallThickness, $"WallN {x},{z}");

            if (grid.IsWallEdge(x, z, Direction.South))
                CreateWall(x, z - 0.5f, 1f, WallThickness, $"WallS {x},{z}");

            if (grid.IsWallEdge(x, z, Direction.East))
                CreateWall(x + 0.5f, z, WallThickness, 1f, $"WallE {x},{z}");

            if (grid.IsWallEdge(x, z, Direction.West))
                CreateWall(x - 0.5f, z, WallThickness, 1f, $"WallW {x},{z}");
        }

        private void CreateWall(float x, float z, float scaleX, float scaleZ, string name)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.parent = _wallParent;
            wall.transform.localPosition = new Vector3(x, WallHeight / 2f, z);
            wall.transform.localScale = new Vector3(scaleX, WallHeight, scaleZ);
            wall.GetComponent<Renderer>().sharedMaterial = _wallMaterial;
            wall.isStatic = true;
        }
    }
}
