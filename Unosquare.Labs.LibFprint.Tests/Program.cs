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
                            var enrollResult = device.EnrollFingerprint("enroll.pgm");
                            if (enrollResult.IsEnrollComplete)
                            {
                                Console.WriteLine("Now, verify your scan just to make sure . . .");
                                var isVerified = device.VerifyFingerprint(enrollResult);
                                if (isVerified)
                                {
                                    enrollCount++;
                                    var printName = "The print " + enrollCount.ToString();
                                    gallery.Add(printName, enrollResult);
                                }
                                else
                                {
                                    Console.WriteLine("Could not verify. Try again!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Try Again -- Error Code {0}", enrollResult.ResultCode);
                            }
                        }

                        while (true)
                        {
                            Console.WriteLine("Press finger against scanner to identify . . .");
                            var identified = device.IdentifyFingerprint(gallery, "identify.pgm");
                            if (identified == null)
                            {
                                Console.WriteLine("Could not identify.");
                            }
                            else
                            {
                                Console.WriteLine("Identified: {0}.", identified);
                            }

                            Console.WriteLine("Pres Q to quit. Any other key to try again.");
                            var consoleKey = Console.ReadKey(true);
                            if (consoleKey.Key == ConsoleKey.Q)
                                break;
                        }


                    }


                    device.Dispose();
                }

            }


        }
    }



}