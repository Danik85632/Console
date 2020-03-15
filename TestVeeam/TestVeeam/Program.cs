using System;
using System.IO;
using System.IO.Compression;
using TestVeeam.Logic;

namespace TestVeeam
{
    class Program
    {
        static GZipBase gZip;
        static int Main(string[] args)
        {
            try
            {
                //args = new string[3];
                //args[0] = @"decompress";
                //args[1] = @"C:\TestVeeam\TestVeeam.gz";
                //args[2] = @"C:\TestVeeam\file1.jpg";

                Check.ChechInputVariables(args);

                switch (args[0].ToLower())
                {
                    case "compress":
                        gZip = new Compressor(args[1], args[2]);
                        break;
                    case "decompress":
                        gZip = new Decompressor(args[1], args[2]);
                        break;
                }
                gZip.Start();
                return gZip.GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error is occured!\n Method: {0}\n Error description {1}\n Press any key to continue.", ex.TargetSite, ex.Message);
                Console.ReadKey();
                return 1;
            }            
        }
    }
}
