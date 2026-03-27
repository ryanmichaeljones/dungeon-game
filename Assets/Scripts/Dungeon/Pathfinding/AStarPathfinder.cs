using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class AStarPathfinder
    {
        public static List<Coordinate> Solve(Coordinate startPosition, Coordinate endPosition, int dungeonRadius)
        {
            PathNode[,] grid = CreateGrid(startPosition, endPosition, dungeonRadius);
            SortedSet<PathNode> openSet = new(Comparer<PathNode>.Create((a, b) =>
            {
                int cmp = a.FCost.CompareTo(b.FCost);
                if (cmp != 0) return cmp;
                cmp = a.Coordinate.X.CompareTo(b.Coordinate.X);
                if (cmp != 0) return cmp;
                return a.Coordinate.Z.CompareTo(b.Coordinate.Z);
            }));
            HashSet<PathNode> closedSet = new();

            PathNode startNode = grid[startPosition.X, startPosition.Z];
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathNode position = openSet.Min;
                openSet.Remove(position);
                closedSet.Add(position);
                bool pathFound = position.Coordinate.X == endPosition.X && position.Coordinate.Z == endPosition.Z;

                if (pathFound)
                {
                    openSet.Clear();

                    List<Coordinate> path = new();

                    while (position.Coordinate.X != startPosition.X || position.Coordinate.Z != startPosition.Z)
                    {
                        path.Add(new Coordinate(position.Coordinate.X, position.Coordinate.Z));
                        position = position.Parent;
                    }

                    path.Add(new Coordinate(startPosition.X, startPosition.Z));
                    path.Reverse();

                    return path;
                }
                else
                {
                    List<PathNode> neighbourPositions = GetCurrentNeighbours(position, grid, dungeonRadius);

                    foreach (PathNode neighbourPosition in neighbourPositions)
                    {
                        bool notExistingPosition = openSet.Contains(neighbourPosition) == false;
                        bool notVisitedPosition = closedSet.Contains(neighbourPosition) == false;

                        if (notExistingPosition && notVisitedPosition)
                        {
                            InvestigateNewPath(grid, openSet, position, neighbourPosition);
                        }
                        else if (notVisitedPosition)
                        {
                            TraverseExistingPath(grid, openSet, position, neighbourPosition);
                        }
                    }
                }
            }

            return new List<Coordinate>();
        }

        private static void TraverseExistingPath(PathNode[,] grid, SortedSet<PathNode> openSet, PathNode position, PathNode neighbourPosition)
        {
            int currentGCost = position.GCost + 1;
            if (currentGCost < neighbourPosition.GCost)
            {
                bool wasInOpenSet = openSet.Remove(neighbourPosition);

                neighbourPosition.GCost = currentGCost;
                neighbourPosition.FCost = neighbourPosition.GCost + neighbourPosition.HCost;
                neighbourPosition.Parent = position;

                grid[neighbourPosition.Coordinate.X, neighbourPosition.Coordinate.Z] = neighbourPosition;

                if (wasInOpenSet)
                {
                    openSet.Add(neighbourPosition);
                }
            }
        }

        private static void InvestigateNewPath(PathNode[,] grid, SortedSet<PathNode> openSet, PathNode position, PathNode neighbourPosition)
        {
            neighbourPosition.GCost = position.GCost + 1;
            neighbourPosition.FCost = neighbourPosition.GCost + neighbourPosition.HCost;
            neighbourPosition.Parent = position;

            grid[neighbourPosition.Coordinate.X, neighbourPosition.Coordinate.Z] = neighbourPosition;

            openSet.Add(neighbourPosition);
        }

        private static PathNode[,] CreateGrid(Coordinate startPosition, Coordinate endPosition, int dungeonRadius)
        {
            PathNode[,] grid = new PathNode[dungeonRadius, dungeonRadius];

            for (int x = 0; x < dungeonRadius; x++)
            {
                for (int z = 0; z < dungeonRadius; z++)
                {
                    PathNode position = new(x, z);
                    grid[x, z] = position;

                    CalculatePositionCosts(position, startPosition, endPosition);
                }
            }

            return grid;
        }

        private static void CalculatePositionCosts(PathNode position, Coordinate startPosition, Coordinate endPosition)
        {
            float distanceStart = Coordinate.CalculateEuclideanDistance(startPosition, position.Coordinate);
            float distanceEnd = Coordinate.CalculateEuclideanDistance(position.Coordinate, endPosition);
            position.GCost = Mathf.RoundToInt(distanceStart);
            position.HCost = Mathf.RoundToInt(distanceEnd);
            position.FCost = position.GCost + position.HCost;
        }

        private static List<PathNode> GetCurrentNeighbours(PathNode currentPosition, PathNode[,] grid, int dungeonRadius)
        {
            List<PathNode> neighbours = new();

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int neighbourX = currentPosition.Coordinate.X + x;
                    int neighbourZ = currentPosition.Coordinate.Z + z;
                    bool validBoundsX = neighbourX >= 0 && neighbourX < dungeonRadius;
                    bool validBoundsZ = neighbourZ >= 0 && neighbourZ < dungeonRadius;

                    if (validBoundsX && validBoundsZ)
                    {
                        PathNode position = grid[neighbourX, neighbourZ];
                        neighbours.Add(position);
                    }
                }
            }

            neighbours.Remove(currentPosition);

            return neighbours;
        }
    }
}
