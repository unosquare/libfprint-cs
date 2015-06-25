using System;
using Unosquare.Labs.LibFprint;

namespace Unosquare.Labs.LibFprint.Tests
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Example taken from: https://github.com/anarsoul/libfprint/blob/master/examples/img_capture.c

            using (var manager = FingerprintDeviceManager.Instance)
            {
                manager.Initialize();
                var devices = manager.DiscoverDevices();

                foreach (var device in devices)
                {
                    device.Open();
                    Console.WriteLine();
                    Console.WriteLine("Device {0} - {1}", device.DriverName, device.DriverFullName);
                    Console.WriteLine("    Enroll Stages:      {0}", device.EnrollStagesCount);
                    Console.WriteLine("    Supports Imaging:   {0}", device.SupportsImaging);
                    Console.WriteLine("    Supports Ident:     {0}", device.SupportsIdentification);
                    Console.WriteLine("    Imaging Dimensions: {0}x{1}", device.ImageWidth, device.ImageHeight);

                    device.Enroll();
                    device.Dispose();
                }

            }


        }
    }



}