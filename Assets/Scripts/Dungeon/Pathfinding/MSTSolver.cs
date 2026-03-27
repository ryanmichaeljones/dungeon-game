using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class MSTSolver
    {
        public static List<(Coordinate From, Coordinate To)> Solve(ReadOnlyCollection<Coordinate> roomPositions)
        {
            int[,] roomGraph = GenerateRoomGraph(roomPositions);
            int vertices = roomPositions.Count;
            int[] parents = new int[vertices];
            int[] indexes = Enumerable.Repeat(int.MaxValue, vertices).ToArray();
            bool[] visited = new bool[vertices];

            indexes[0] = 0;
            parents[0] = -1;

            for (int i = 0; i < vertices - 1; i++)
            {
                int index = GetClosestIndex(indexes, visited, vertices);
                visited[index] = true;

                for (int v = 0; v < vertices; v++)
                {
                    int roomNode = roomGraph[index, v];
                    bool isChildNode = roomNode < indexes[v];

                    if (roomNode != 0 && visited[v] == false && isChildNode)
                    {
                        parents[v] = index;
                        indexes[v] = roomNode;
                    }
                }
            }

            List<(Coordinate From, Coordinate To)> paths = GeneratePaths(roomPositions, vertices, parents);
            return paths;
        }

        private static List<(Coordinate From, Coordinate To)> GeneratePaths(ReadOnlyCollection<Coordinate> roomPositions, int vertices, int[] parents)
        {
            List<(Coordinate From, Coordinate To)> paths = new(vertices);

            for (int v = 1; v < vertices; v++)
            {
                int parent = parents[v];
                Coordinate parentPosition = roomPositions[parent];
                Coordinate nextPosition = roomPositions[v];
                (Coordinate parentPosition, Coordinate nextPosition) path = (parentPosition, nextPosition);
                paths.Add(((Coordinate From, Coordinate To))path);
            }

            return paths;
        }

        private static int[,] GenerateRoomGraph(ReadOnlyCollection<Coordinate> roomPositions)
        {
            int[,] graph = new int[roomPositions.Count, roomPositions.Count];

            for (int r = 0; r < roomPositions.Count; r++)
            {
                for (int c = r + 1; c < roomPositions.Count; c++)
                {
                    int distance = Mathf.RoundToInt(Coordinate.CalculateEuclideanDistance(roomPositions[r], roomPositions[c]));
                    graph[r, c] = distance;
                    graph[c, r] = distance;
                }
            }

            return graph;
        }

        private static int GetClosestIndex(int[] indexes, bool[] visited, int vertices)
        {
            int minVertex = int.MaxValue, minIndex = -1;

            for (int v = 0; v < vertices; v++)
            {
                if (visited[v] == false && indexes[v] < minVertex)
                {
                    minVertex = indexes[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }
    }
}
