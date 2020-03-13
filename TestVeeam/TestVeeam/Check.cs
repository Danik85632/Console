using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestVeeam
{
    public static class Check
    {
        public static void ReadInputVariables(string[] args)
        {
            if (args.Length == 0 || args.Length > 3)
            {
                throw new Exception("The program parameters, the names of the source and resulting files should be set on the command line as follows:\n" +
                    "compress/decompress [source file path] [result file path].");
            }

            if (args[0].ToLower() != "compress" && args[0].ToLower() != "decompress")
            {
                throw new Exception("The first argument shall be \"compress\" or \"decompress\".");
            }

            if (args[1].Length == 0)
            {
                throw new Exception("No source file name was specified.");
            }

            if (!File.Exists(args[1]))
            {
                throw new Exception("No source file was found.");
            }

            if (args[1] == args[2])
            {
                throw new Exception("Source and destination files must be different.");
            }

            FileInfo FileFrom = new FileInfo(args[1]);
            FileInfo FileTo = new FileInfo(args[2]);

            if (FileFrom.Extension == ".gz" && args[0] == "compress")
            {
                throw new Exception("The file has already been compressed.");
            }

            if (FileFrom.Extension != ".gz" && args[0] == "decompress")
            {
                throw new Exception("The unzip file must have the extension .gz.");
            }

            if (FileTo.Extension == ".gz" && FileTo.Exists)
            {
                throw new Exception("The destination file already exists.Please provide a different file name.");
            }

            if (args[2].Length == 0)
            {
                throw new Exception("The destination file name was not specified.");
            }
        }
    }
}
