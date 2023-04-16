
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MatrixVision.Connector.Core;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");

ConfigurationAccessor.Configuration = configuration.Build();

Console.WriteLine(ConfigurationAccessor.FolderPath);

_ = BenchmarkRunner.Run<SingleCaptureBenchmark>();

[MemoryDiagnoser]
public class SingleCaptureBenchmark
{
    readonly string DEVICE_ID = "VD000001";
    readonly string PRE_CONNECTION_DEVICE_ID = "VD000002";
    readonly string IMAGE_FORMAT = "png";

    private static ConnectionManager connectionManager = new();

    [GlobalSetup]
    public void GlobalSetup()
    {
        connectionManager.ConnectCamera(PRE_CONNECTION_DEVICE_ID);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        connectionManager.DisconnectCamera(PRE_CONNECTION_DEVICE_ID);
    }

    [Benchmark]
    public void SingularCapture()
    {
        using var connector = new MVConnector();

        _ = connector.CaptureSingle(DEVICE_ID, IMAGE_FORMAT, ConfigurationAccessor.VirtualDeviceEnabled);
    }

    [Benchmark]
    public void PreConnectCaptureWithoutConnection()
    {
        connectionManager.ConnectCamera(DEVICE_ID);

        var connector = connectionManager.GetCameraConnection(DEVICE_ID);

        _ = connector.Capture(IMAGE_FORMAT);

        connectionManager.DisconnectCamera(DEVICE_ID);
    }

    [Benchmark]
    public void PreConnectCaptureWithConnection()
    {
        var connector = connectionManager.GetCameraConnection(PRE_CONNECTION_DEVICE_ID);

        _ = connector.Capture(IMAGE_FORMAT);
    }
}