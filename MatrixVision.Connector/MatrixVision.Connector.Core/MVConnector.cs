using mv.impact.acquire;
using mv.impact.acquire.examples.helper;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace MatrixVision.Connector.Core
{
    public class MVConnector : IDisposable
    {
        const int TIMEOUT_MS = 10000;

        private Device? device;
        private FunctionInterface? functionInterface;

        public bool IsConnected { get; private set; }

        public MVConnector()
        {
            LibraryPath.init();
        }

        public Response CaptureSingle(string serialId, string format, bool virtualDevice = false)
        {
            ConfigureDevice(serialId, virtualDevice);

            var imageFormat = ParseImageFormat(format);

            if (imageFormat is null)
            {
                throw new ArgumentException($"Unsupported format requested. Format: {format}");
            }

            return CaptureFrame(imageFormat!);
        }

        public void Connect(string serialId, bool virtualDevice = false)
        {
            ConfigureDevice(serialId, virtualDevice);
        }

        public void Disconnect()
        {
            if (device != null)
            {
                if (functionInterface != null)
                {
                    functionInterface = null;
                }

                DeviceAccess.manuallyStopAcquisitionIfNeeded(device, functionInterface);
                device.close();
                device = null;
            }
        }

        public Response Capture(string format)
        {
            var imageFormat = ParseImageFormat(format);

            if (imageFormat is null)
            {
                throw new ArgumentException($"Unsupported format requested. Format: {format}");
            }

            if (device is null || functionInterface is null)
            {
                throw new InvalidOperationException("Device is not connected.");
            }

            return CaptureFrame(imageFormat!);
        }

        private void ConfigureDevice(string serialId, bool virtualDevice)
        {
            if (device is not null)
            {
                throw new InvalidOperationException($"Device is already set up. SerialId: {serialId}");
            }

            // Find the device with the given serial ID
            device = DeviceManager.getDeviceBySerial(serialId);

            if (device.isInUse)
            {
                IsConnected = false;

                throw new InvalidOperationException($"Device is in use. SerialId: {serialId}");
            }

            device.open();

            functionInterface = new(device);

            IsConnected = true;

            if (virtualDevice)
            {
                var virtualDeviceSettings = new CameraSettingsVirtualDevice(device);

                var path = ConfigurationAccessor.FolderPath;

                virtualDeviceSettings.testMode.write(TVirtualDeviceTestMode.vdtmImageDirectory);
                virtualDeviceSettings.imageDirectory.write(path);

                virtualDeviceSettings.imageType.write(TVirtualDeviceImageType.vditPNG);
                virtualDeviceSettings.aoiMode.write(TCameraAoiMode.camFull);
            }
        }

        private Response CaptureFrame(ImageFormat imageFormat)
        {
            TDMR_ERROR result = (TDMR_ERROR)functionInterface!.imageRequestSingle();

            if (result != TDMR_ERROR.DMR_NO_ERROR)
            {
                throw new InvalidOperationException($"'FunctionInterface.imageRequestSingle' returned with an unexpected result: {result} (Error code: {ImpactAcquireException.getErrorCodeAsString(result)})");
            }

            DeviceAccess.manuallyStartAcquisitionIfNeeded(device, functionInterface);

            int requestNr = functionInterface.imageRequestWaitFor(TIMEOUT_MS);

            if (functionInterface.isRequestNrValid(requestNr))
            {
                var request = functionInterface.getRequest(requestNr);

                Response response;

                if (request.isOK)
                {
                    using RequestBitmapData bitmapData = request.bitmapData;
                    using var ms = new MemoryStream();

                    bitmapData.bitmap.Save(ms, imageFormat as ImageFormat);

                    var image = new Asset
                    {
                        CameraId = device!.serial.readS(),
                        Image = Convert.ToBase64String(ms.GetBuffer())
                    };

                    response = Response.CapturedSingle(image);
                }
                else
                {
                    response = Response.Error($"Error: {request.requestResult.readS()}");
                }

                request!.unlock();
                return response;
            }
            else
            {
                return Response.Error("Request timeout");
            }
        }

        private static ImageFormat? ParseImageFormat(string format) => 
            (ImageFormat?)typeof(ImageFormat)
                    .GetProperty(format, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)?
                    .GetValue(format, null);

        public void Dispose()
        {
            Disconnect();
        }
    }
}

