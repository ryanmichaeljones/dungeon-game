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
                    List<PathNode> neighbourNodes = GetCurrentNeighbours(node, grid, dungeonRadius);
                    InvestigateNeighbours(grid, openSet, closedSet, node, neighbourNodes);
                }
            }

            return new List<Coordinate>();
        }

        private static void InvestigateNeighbours(PathNode[,] grid, SortedSet<PathNode> openSet, HashSet<PathNode> closedSet, PathNode node, List<PathNode> neighbourNodes)
        {
            foreach (PathNode neighbourNode in neighbourNodes)
            {
                bool notExistingNode = openSet.Contains(neighbourNode) == false;
                bool notVisitedNode = closedSet.Contains(neighbourNode) == false;

                if (notExistingNode && notVisitedNode)
                {
                    InvestigateNewPath(grid, openSet, node, neighbourNode);
                }
                else if (notVisitedNode)
                {
                    TraverseExistingPath(grid, openSet, node, neighbourNode);
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

        private static void TraverseExistingPath(PathNode[,] grid, SortedSet<PathNode> openSet, PathNode node, PathNode neighbourNode)
        {
            int currentGCost = node.GCost + 1;
            if (currentGCost < neighbourNode.GCost)
            {
                bool wasInOpenSet = openSet.Remove(neighbourNode);

                neighbourNode.GCost = currentGCost;
                neighbourNode.FCost = neighbourNode.GCost + neighbourNode.HCost;
                neighbourNode.Parent = node;

                grid[neighbourNode.Coordinate.X, neighbourNode.Coordinate.Z] = neighbourNode;

                if (wasInOpenSet)
                {
                    openSet.Add(neighbourNode);
                }
            }
        }

        private static void InvestigateNewPath(PathNode[,] grid, SortedSet<PathNode> openSet, PathNode node, PathNode neighbourNode)
        {
            neighbourNode.GCost = node.GCost + 1;
            neighbourNode.FCost = neighbourNode.GCost + neighbourNode.HCost;
            neighbourNode.Parent = node;

            grid[neighbourNode.Coordinate.X, neighbourNode.Coordinate.Z] = neighbourNode;

            openSet.Add(neighbourNode);
        }

        private static PathNode[,] CreateGrid(Coordinate startPosition, Coordinate endPosition, int dungeonRadius)
        {
            PathNode[,] grid = new PathNode[dungeonRadius, dungeonRadius];

            for (int x = 0; x < dungeonRadius; x++)
            {
                for (int z = 0; z < dungeonRadius; z++)
                {
                    PathNode node = new(x, z);
                    grid[x, z] = node;

                    CalculatePositionCosts(node, startPosition, endPosition);
                }
            }

            return grid;
        }

        private static void CalculatePositionCosts(PathNode node, Coordinate startPosition, Coordinate endPosition)
        {
            float distanceStart = Coordinate.CalculateEuclideanDistance(startPosition, node.Coordinate);
            float distanceEnd = Coordinate.CalculateEuclideanDistance(node.Coordinate, endPosition);
            node.GCost = Mathf.RoundToInt(distanceStart);
            node.HCost = Mathf.RoundToInt(distanceEnd);
            node.FCost = node.GCost + node.HCost;
        }

        private static List<PathNode> GetCurrentNeighbours(PathNode currentNode, PathNode[,] grid, int dungeonRadius)
        {
            List<PathNode> neighbourNodes = new();

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int neighbourX = currentNode.Coordinate.X + x;
                    int neighbourZ = currentNode.Coordinate.Z + z;
                    bool validBoundsX = neighbourX >= 0 && neighbourX < dungeonRadius;
                    bool validBoundsZ = neighbourZ >= 0 && neighbourZ < dungeonRadius;

                    if (validBoundsX && validBoundsZ)
                    {
                        PathNode node = grid[neighbourX, neighbourZ];
                        neighbourNodes.Add(node);
                    }
                }
            }

            neighbourNodes.Remove(currentNode);

            return neighbourNodes;
        }
    }
}
