using System;
using System.Collections.Concurrent;

namespace MatrixVision.Connector.Core
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<string, MVConnector> connectedCameras;

        public ConnectionManager()
        {
            connectedCameras = new ConcurrentDictionary<string, MVConnector>();
        }

        public string ConnectCamera(string serialId)
        {
            var cameraConnector = new MVConnector();
            cameraConnector.Connect(serialId, ConfigurationAccessor.VirtualDeviceEnabled);

            connectedCameras.TryAdd(serialId, cameraConnector);

            return serialId;
        }

        public void DisconnectCamera(string serialId)
        {
            if (connectedCameras.TryRemove(serialId, out MVConnector cameraConnector))
            {
                cameraConnector.Disconnect();
            }
            else
            {
                throw new ArgumentException($"Unknown serial ID. SerialId: {serialId}");
            }
        }

        public MVConnector GetCameraConnection(string serialId)
        {
            if (connectedCameras.TryGetValue(serialId, out MVConnector cameraConnector))
            {
                return cameraConnector;
            }
            else
            {
                throw new ArgumentException($"Unknown serial ID. SerialId: {serialId}");
            }
        }
    }
}