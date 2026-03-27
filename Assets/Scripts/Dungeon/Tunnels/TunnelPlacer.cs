using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class TunnelPlacer
    {
        private readonly Tunnel _tunnelPrefab;
        private readonly Transform _tunnelsParent;

        private const float TunnelPositionY = 0.0f;
        private const float TunnelOffset = 0.05f;

        public TunnelPlacer(Tunnel tunnelPrefab, Transform tunnelsParent)
        {
            _tunnelPrefab = tunnelPrefab;
            _tunnelsParent = tunnelsParent;
        }

        public void PlaceTunnels(List<Coordinate> path, Func<float, float, bool> isInsideRoom)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Coordinate from = path[i];
                Coordinate to = path[i + 1];

                for (float t = 0; t < 1; t += TunnelOffset)
                {
                    Vector3 vector = new(to.X - from.X, 0.0f, to.Z - from.Z);
                    float tunnelX = from.X + t * vector.x;
                    float tunnelZ = from.Z + t * vector.z;

                    if (!isInsideRoom(tunnelX, tunnelZ))
                    {
                        CreateTunnel(from.X, from.Z, t * vector);
                    }
                }
            }
        }

        private void CreateTunnel(int x, int z, Vector3 offset)
        {
            Tunnel newTunnel = Object.Instantiate(_tunnelPrefab);
            float positionX = x + offset.x;
            float positionZ = z + offset.z;

            newTunnel.name = $"{x}, {z}";
            newTunnel.transform.parent = _tunnelsParent;
            newTunnel.transform.localPosition = new Vector3(positionX, TunnelPositionY, positionZ);
        }
    }
}
