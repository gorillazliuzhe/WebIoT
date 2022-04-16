using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Unosquare.RaspberryIO;
//using Unosquare.RaspberryIO.Camera;

namespace WebIoT.Playground.Media
{
    public class CaptureClient
    {
        static void TestCaptureVideo()
        {
            // Setup our working variables
            var videoByteCount = 0;
            var videoEventCount = 0;
            var startTime = DateTime.UtcNow;

            // Configure video settings
            //var videoSettings = new CameraVideoSettings()
            //{
            //    CaptureTimeoutMilliseconds = 0,
            //    CaptureDisplayPreview = false,
            //    ImageFlipVertically = true,
            //    CaptureExposure = CameraExposureMode.Night,
            //    CaptureWidth = 1920,
            //    CaptureHeight = 1080
            //};

            try
            {
                // Start the video recording
                //Pi.Camera.OpenVideoStream(videoSettings,
                //    onDataCallback: (data) => { videoByteCount += data.Length; videoEventCount++; },
                //    onExitCallback: null);

                // Wait for user interaction
                startTime = DateTime.UtcNow;
                Console.WriteLine("Press any key to stop reading the video stream . . .");
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
            }
            finally
            {
                // Always close the video stream to ensure raspivid quits
                //Pi.Camera.CloseVideoStream();

                // Output the stats
                var megaBytesReceived = (videoByteCount / (1024f * 1024f)).ToString("0.000");
                var recordedSeconds = DateTime.UtcNow.Subtract(startTime).TotalSeconds.ToString("0.000");
                Console.WriteLine($"Capture Stopped. Received {megaBytesReceived} Mbytes in {videoEventCount} callbacks in {recordedSeconds} seconds");
            }
        }
    }
}
