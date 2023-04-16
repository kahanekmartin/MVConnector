# MVConnector
REST API Connector for Matrix Vision devices

## Prerequisites
- .NET 6 or later
- [FreeImage](https://freeimage.sourceforge.io/index.html)
- [Postman](https://www.postman.com/)

## Matrix Vision .NET SDK
- [Download the .NET SDK for Matrix Vision cameras](https://www.matrix-vision.com/en/product-line/software/mvimpact-acquire-sdk)
- [.NET SDK Reference](https://www.matrix-vision.com/manuals/SDK_NET/)

## Benchmark Results
``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 10 (10.0.19045.2728/22H2/2022Update)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.300
  [Host]     : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 6.0.5 (6.0.522.21309), X64 RyuJIT AVX2

```
|                             Method |      Mean |    Error |   StdDev | Allocated |
|----------------------------------- |----------:|---------:|---------:|----------:|
|                    SingularCapture | 428.97 ms | 1.034 ms | 0.967 ms | 147.52 KB |
| PreConnectCaptureWithoutConnection | 429.72 ms | 1.055 ms | 0.935 ms | 147.63 KB |
|    PreConnectCaptureWithConnection |  22.00 ms | 0.151 ms | 0.134 ms |  112.6 KB |

- SingularCapture creates new connection with the reuquested device
- PreConnectCaptureWithoutConnection connects/disconnects to the requested device with dedicated endpoints
- PreConnectCaptureWithConnection uses previously created connection with requested device

## Testing Images Dataset
- [Download the testing images dataset](https://www.kaggle.com/datasets/intelecai/car-segmentation)

## Configuration Steps
1. Add the Matrix Vision .NET SDK to your project.
2. Extract the testing images dataset to your project folder.
3. Install FreeImage on your device
4. Replace folder path for testing images in appsettings.json:
```json
  "VirtualDeviceSettings": {
    "Enabled": true,
    "ImagesFolderPath": "path/to/images/folder"
  }
```
5. Import attached Postman collection (replace loclahost port according to your launchSettings setup)

## Notes
- Batch querying is in development (current implementation only for testing purposes)
