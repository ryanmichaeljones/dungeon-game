namespace Assets.Scripts
{
    public class PathNode
    {
        public readonly Coordinate Coordinate;

        public int GCost;
        public int HCost;
        public int FCost;

        public PathNode Parent;

        public PathNode(int x, int z)
        {
            Coordinate = new Coordinate(x, z);
        }
    }
}
