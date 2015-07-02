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
                Console.WriteLine("Initialized Device Manager.");
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

                    using (var gallery = new FingerprintGallery())
                    {
                        var enrollCount = 0;
                        while (enrollCount < 5)
                        {
                            Console.WriteLine("Enroll count: {0}. Enroll a new finger now.", enrollCount);
                            var enrollResult = device.ExecuteEnrollStage();
                            if (enrollResult.IsEnrollComplete)
                            {
                                enrollCount++;
                                gallery.Add("print" + enrollCount.ToString(), enrollResult.FingerprintData);
                            }
                            else
                            {
                                Console.WriteLine("Try Again -- Error Code {0}", enrollResult.ResultCode);
                            }
                        }

                        Console.WriteLine("Enrollment complete. Now let's identify . . .");
                        var identified = device.IdentifyFingerprint(gallery);
                        if (identified == null)
                        {
                            Console.WriteLine("Could not identify.");
                        }
                        else
                        {
                            Console.WriteLine("Identified: {0}", identified);
                        }

                    }


                    device.Dispose();
                }

            }


        }
    }



}