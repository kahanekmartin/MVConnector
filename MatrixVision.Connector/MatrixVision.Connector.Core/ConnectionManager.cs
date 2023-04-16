using System;
using System.Collections.Concurrent;

namespace MatrixVision.Connector.Core
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<string, MVConnector> _connectedCameras;

        public ConnectionManager()
        {
            _connectedCameras = new ConcurrentDictionary<string, MVConnector>();
        }

        public string ConnectCamera(string serialId)
        {
            var cameraConnector = new MVConnector();
            cameraConnector.Connect(serialId, true);

            _connectedCameras.TryAdd(serialId, cameraConnector);

            return serialId;
        }

        public void DisconnectCamera(string serialId)
        {
            if (_connectedCameras.TryRemove(serialId, out MVConnector cameraConnector))
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
            if (_connectedCameras.TryGetValue(serialId, out MVConnector cameraConnector))
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