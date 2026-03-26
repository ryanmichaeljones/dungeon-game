using System;

public class Coordinate : IEquatable<Coordinate>
{
    public readonly int X;
    public readonly int Z;

    public int GCost;
    public int HCost;
    public int FCost;

    public Coordinate Parent;

    public Coordinate(int x, int z)
    {
        X = x;
        Z = z;
    }

    public bool Equals(Coordinate other) => other != null 
        && X == other.X 
        && Z == other.Z;
}
