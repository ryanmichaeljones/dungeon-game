using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class AStarPathfinder
    {
        public static List<Coordinate> Solve(Coordinate startPosition, Coordinate endPosition, int dungeonRadius)
        {
            PathNode[,] grid = CreateGrid(startPosition, endPosition, dungeonRadius);
            SortedSet<PathNode> openSet = CreateOpenSet();
            HashSet<PathNode> closedSet = new();
            PathNode startNode = grid[startPosition.X, startPosition.Z];
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathNode node = openSet.Min;
                openSet.Remove(node);
                closedSet.Add(node);
                bool pathFound = node.Coordinate.X == endPosition.X && node.Coordinate.Z == endPosition.Z;

                if (pathFound)
                {
                    openSet.Clear();
                    List<Coordinate> path = GeneratePathSolution(startPosition, ref node);
                    return path;
                }
                else
                {
                    List<PathNode> neighbourPositions = GetCurrentNeighbours(node, grid, dungeonRadius);
                    InvestigateNeighbours(grid, openSet, closedSet, node, neighbourPositions);
                }
            }

            return new List<Coordinate>();
        }

        private static void InvestigateNeighbours(PathNode[,] grid, SortedSet<PathNode> openSet, HashSet<PathNode> closedSet, PathNode node, List<PathNode> neighbourPositions)
        {
            foreach (PathNode neighbourPosition in neighbourPositions)
            {
                bool notExistingPosition = openSet.Contains(neighbourPosition) == false;
                bool notVisitedPosition = closedSet.Contains(neighbourPosition) == false;

                if (notExistingPosition && notVisitedPosition)
                {
                    InvestigateNewPath(grid, openSet, node, neighbourPosition);
                }
                else if (notVisitedPosition)
                {
                    TraverseExistingPath(grid, openSet, node, neighbourPosition);
                }
            }
        }

        private static List<Coordinate> GeneratePathSolution(Coordinate startPosition, ref PathNode node)
        {
            List<Coordinate> path = new();

            while (node.Coordinate.X != startPosition.X || node.Coordinate.Z != startPosition.Z)
            {
                path.Add(new Coordinate(node.Coordinate.X, node.Coordinate.Z));
                node = node.Parent;
            }

            path.Add(new Coordinate(startPosition.X, startPosition.Z));
            path.Reverse();
            return path;
        }

        private static SortedSet<PathNode> CreateOpenSet() => new(Comparer<PathNode>.Create((a, b) =>
        {
            int cmp = a.FCost.CompareTo(b.FCost);
            if (cmp != 0) return cmp;
            cmp = a.Coordinate.X.CompareTo(b.Coordinate.X);
            if (cmp != 0) return cmp;
            return a.Coordinate.Z.CompareTo(b.Coordinate.Z);
        }));

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
