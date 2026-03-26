using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class Dungeon : MonoBehaviour
{
    [SerializeField] private Room roomPrefab;
    [SerializeField] private Tunnel tunnelPrefab;
    [SerializeField] private Transform roomsParent;
    [SerializeField] private Transform tunnelsParent;

    private const int RoomOverlapThreshold = 2; // consider the dungeon room size from coordinate
    private const float MinRoomWidth = 1.5f;
    private const float MaxRoomWidth = 2.5f;
    private const float MinRoomHeight = 1.5f;
    private const float MaxRoomHeight = 2.5f;
    private const float RoomScaleY = 0.2f;
    private const float RoomPositionY = 0.0f;
    private const float TunnelPositionY = 0.0f;
    private const float TunnelOffset = 0.05f;

    // consider if we can change how tunnels are generated to reduce how many objects are instantiated - changing their rotation perhaps - performance impact is significant!
    // change values to SerializeField to edit in inspector
    // consider changing to weighted room sizes - rarer case of larger room

    private readonly List<Rect> _roomBounds = new();

    public void Initialize(int roomCount, int dungeonRadius)
    {
        _roomBounds.Clear(); //don't like this

        ReadOnlyCollection<Coordinate> roomPositions = CreateRoomPositions(roomCount, dungeonRadius);
        int[,] roomGraph = GenerateRoomGraph(roomPositions);

        SolveMST(roomPositions, roomGraph, dungeonRadius);
    }

    private ReadOnlyCollection<Coordinate> CreateRoomPositions(int roomCount, int dungeonRadius)
    {
        List<Coordinate> validPositions = GetValidRoomPositions(dungeonRadius);
        List<Coordinate> roomPositions = new();

        for (int i = 0; i < roomCount; i++)
        {
            if (validPositions.Count == 0)
            {
                Debug.LogWarning($"No valid positions remaining. Placed {roomPositions.Count}/{roomCount} rooms.");
                break;
            }

            int index = Random.Range(0, validPositions.Count);
            Coordinate position = validPositions[index];
            roomPositions.Add(position);
            validPositions.RemoveAll(p => EvaluatePositionOverlap(p, position));

            CreateRoom(position.X, position.Z);
        }

        return roomPositions.AsReadOnly();
    }

    private static List<Coordinate> GetValidRoomPositions(int dungeonRadius)
    {
        List<Coordinate> validPositions = new();

        for (int x = 0; x < dungeonRadius; x++)
        {
            for (int z = 0; z < dungeonRadius; z++)
            {
                Coordinate position = new(x, z);
                validPositions.Add(position);
            }
        }

        return validPositions;
    }

    private static bool EvaluatePositionOverlap(Coordinate evaluatePosition, Coordinate newPosition)
    {
        int deltaX = Mathf.Abs(evaluatePosition.X - newPosition.X);
        int deltaZ = Mathf.Abs(evaluatePosition.Z - newPosition.Z);
        return deltaX <= RoomOverlapThreshold && deltaZ <= RoomOverlapThreshold;
    }

    private int[,] GenerateRoomGraph(ReadOnlyCollection<Coordinate> roomPositions)
    {
        int[,] graph = new int[roomPositions.Count, roomPositions.Count];

        for (int r = 0; r < roomPositions.Count; r++)
        {
            for (int c = r + 1; c < roomPositions.Count; c++)
            {
                int distance = Mathf.RoundToInt(CalculateEuclideanDistance(roomPositions[r], roomPositions[c]));
                graph[r, c] = distance;
                graph[c, r] = distance;
            }
        }

        return graph;
    }

    private void CreateRoom(int x, int z)
    {
        Room newRoom = Instantiate(roomPrefab);
        float width = Random.Range(MinRoomWidth, MaxRoomWidth);
        float height = Random.Range(MinRoomHeight, MaxRoomHeight);
        float scaleX = newRoom.transform.localScale.x * width;
        float scaleZ = newRoom.transform.localScale.z * height;

        newRoom.name = $"{x}, {z}";
        newRoom.transform.parent = roomsParent;
        newRoom.transform.localScale = new Vector3(scaleX, RoomScaleY, scaleZ);
        newRoom.transform.localPosition = new Vector3(x, RoomPositionY, z);

        _roomBounds.Add(new Rect(x - scaleX / 2f, z - scaleZ / 2f, scaleX, scaleZ));
    }

    private void CreateTunnel(int x, int z, Vector3 offset)
    {
        Tunnel newTunnel = Instantiate(tunnelPrefab);
        float positionX = x + offset.x;
        float positionZ = z + offset.z;

        newTunnel.name = $"{x}, {z}";
        newTunnel.transform.parent = tunnelsParent;
        newTunnel.transform.localPosition = new Vector3(positionX, TunnelPositionY, positionZ);
    }

    private static float CalculateEuclideanDistance(Coordinate a, Coordinate b) => Mathf.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z));

    private void SolveMST(ReadOnlyCollection<Coordinate> roomPositions, int[,] roomGraph, int dungeonRadius)
    {
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

        for (int v = 1; v < vertices; v++)
        {
            int parent = parents[v];
            Coordinate parentPosition = roomPositions[parent];
            Coordinate nextPosition = roomPositions[v];

            SolveAStar(parentPosition, nextPosition, dungeonRadius);
        }
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

    private void SolveAStar(Coordinate startPosition, Coordinate endPosition, int dungeonRadius)
    {
        Coordinate[,] grid = CreateGrid(startPosition, endPosition, dungeonRadius);
        SortedSet<Coordinate> openSet = new(Comparer<Coordinate>.Create((a, b) =>
        {
            int cmp = a.FCost.CompareTo(b.FCost);
            if (cmp != 0) return cmp;
            cmp = a.X.CompareTo(b.X);
            if (cmp != 0) return cmp;
            return a.Z.CompareTo(b.Z);
        }));
        HashSet<Coordinate> closedSet = new();

        openSet.Add(startPosition);

        while (openSet.Count > 0)
        {
            Coordinate position = openSet.Min;
            openSet.Remove(position);
            closedSet.Add(position);
            bool pathFound = position.X == endPosition.X && position.Z == endPosition.Z;

            if (pathFound)
            {
                openSet.Clear();

                while (position != startPosition)
                {
                    Coordinate previousPosition = position;
                    position = position.Parent;

                    for (float i = 0; i < 1; i += TunnelOffset)
                    {
                        Vector3 vector = new(position.X - previousPosition.X, 0.0f, position.Z - previousPosition.Z);
                        float tunnelX = previousPosition.X + i * vector.x;
                        float tunnelZ = previousPosition.Z + i * vector.z;

                        if (!IsInsideAnyRoom(tunnelX, tunnelZ))
                        {
                            CreateTunnel(previousPosition.X, previousPosition.Z, i * vector);
                        }
                    }
                }
            }
            else
            {
                List<Coordinate> neighbourPositions = GetCurrentNeighbours(position, grid, dungeonRadius);

                foreach (Coordinate neighbourPosition in neighbourPositions)
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
    }

    private static void TraverseExistingPath(Coordinate[,] grid, SortedSet<Coordinate> openSet, Coordinate position, Coordinate neighbourPosition)
    {
        int currentGCost = position.GCost + 1;
        if (currentGCost < neighbourPosition.GCost)
        {
            bool wasInOpenSet = openSet.Remove(neighbourPosition);

            neighbourPosition.GCost = currentGCost;
            neighbourPosition.FCost = neighbourPosition.GCost + neighbourPosition.HCost;
            neighbourPosition.Parent = position;

            grid[neighbourPosition.X, neighbourPosition.Z] = neighbourPosition;

            if (wasInOpenSet)
            {
                openSet.Add(neighbourPosition);
            }
        }
    }

    private static void InvestigateNewPath(Coordinate[,] grid, SortedSet<Coordinate> openSet, Coordinate position, Coordinate neighbourPosition)
    {
        neighbourPosition.GCost = position.GCost + 1;
        neighbourPosition.FCost = neighbourPosition.GCost + neighbourPosition.HCost;
        neighbourPosition.Parent = position;

        grid[neighbourPosition.X, neighbourPosition.Z] = neighbourPosition;

        openSet.Add(neighbourPosition);
    }

    private static Coordinate[,] CreateGrid(Coordinate startPosition, Coordinate endPosition, int dungeonRadius)
    {
        Coordinate[,] grid = new Coordinate[dungeonRadius, dungeonRadius];

        for (int x = 0; x < dungeonRadius; x++)
        {
            for (int z = 0; z < dungeonRadius; z++)
            {
                Coordinate position = new(x, z);
                grid[x, z] = position;

                CalculatePositionCosts(position, startPosition, endPosition);
            }
        }

        return grid;
    }

    private static void CalculatePositionCosts(Coordinate position, Coordinate startPosition, Coordinate endPosition)
    {
        float distanceStart = CalculateEuclideanDistance(startPosition, position);
        float distanceEnd = CalculateEuclideanDistance(position, endPosition);
        position.GCost = Mathf.RoundToInt(distanceStart);
        position.HCost = Mathf.RoundToInt(distanceEnd);
        position.FCost = position.GCost + position.HCost;
    }

    private List<Coordinate> GetCurrentNeighbours(Coordinate currentPosition, Coordinate[,] grid, int dungeonRadius)
    {
        List<Coordinate> neighbours = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                int neighbourX = currentPosition.X + x;
                int neighbourZ = currentPosition.Z + z;
                bool validBoundsX = neighbourX >= 0 && neighbourX < dungeonRadius;
                bool validBoundsZ = neighbourZ >= 0 && neighbourZ < dungeonRadius;

                if (validBoundsX && validBoundsZ)
                {
                    Coordinate position = grid[neighbourX, neighbourZ];
                    neighbours.Add(position);
                }
            }
        }

        neighbours.Remove(currentPosition);

        return neighbours;
    }

    private bool IsInsideAnyRoom(float worldX, float worldZ)
    {
        foreach (Rect bounds in _roomBounds)
        {
            if (bounds.Contains(new Vector2(worldX, worldZ)))
            {
                return true;
            }
        }
        return false;
    }
}
