using System;
using UnityEngine;

namespace Assets.Scripts
{
    public readonly struct Coordinate : IEquatable<Coordinate>
    {
        public readonly int X;
        public readonly int Z;

        public Coordinate(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static float CalculateEuclideanDistance(Coordinate a, Coordinate b) => Mathf.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z));

        public bool Equals(Coordinate other) => X == other.X && Z == other.Z;

        public override bool Equals(object obj) => obj is Coordinate other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Z);
    }
}
