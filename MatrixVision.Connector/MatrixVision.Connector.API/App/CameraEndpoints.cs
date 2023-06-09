﻿using MatrixVision.Connector.Core;
using Microsoft.AspNetCore.Mvc;

namespace MatrixVision.Connector.API.App
{
    public static class CameraEndpoints
    {
        private static ConnectionManager connectionManager = new();

        public static void MapCameraEndpoints(this WebApplication app)
        { 
            app.MapGet("/api/cameras/{deviceid}/connect", ([FromRoute] string deviceId) =>
            {
                connectionManager.ConnectCamera(deviceId);

                return Results.Ok();
            });

            app.MapGet("/api/cameras/{deviceid}/disconnect", ([FromRoute] string deviceId) =>
            {
                connectionManager.DisconnectCamera(deviceId);

                return Results.Ok();
            });

            app.MapGet("/api/capture/single/{format}/{deviceId}/singular", ([FromRoute] string format, [FromRoute] string deviceId) =>
            {
                using var connector = new MVConnector();

                var result = connector.CaptureSingle(deviceId, format, ConfigurationAccessor.VirtualDeviceEnabled);

                return Results.Ok(result);
            });

            app.MapGet("/api/capture/single/{format}/{deviceId}", ([FromRoute] string format, [FromRoute] string deviceId) =>
            {
                var connector = connectionManager.GetCameraConnection(deviceId);

                if (connector.IsConnected)
                {
                    var result = connector.Capture(format);

                    return Results.Ok(result);
                }

                return Results.BadRequest(Response.Error($"Unable to connect to the selected device. DeviceId: {deviceId}"));
            });

            app.MapGet("/api/capture/stream-batch/{format}/{deviceId}/{numImages}/{fps}", async ([FromRoute] string format, [FromRoute] string deviceId, [FromRoute] int numImages, [FromRoute] int fps) =>
            {
                var connector = connectionManager.GetCameraConnection(deviceId);

                if (connector.IsConnected)
                {
                    var result = await connector.CaptureBatch(numImages, fps, format);

                    return Results.Ok(result);
                }

                return Results.BadRequest(Response.Error($"Unable to connect to the selected device. DeviceId: {deviceId}"));
            });
        }
    }
}
