using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravity.DBReaderWriter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args == null || !args.Any())
                {
                    Console.WriteLine("Arguements not provided. Exiting");
                    return;
                }
                if (args.Length == 5)
                {
                    var nodePrefix = args[0];
                    var spName = args[1];
                    var inputFileName = args[2];
                    var outputFileName = args[3];
                    var outputFileType = args[4];

                    var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                    var inputFilePath = exePath + Path.DirectorySeparatorChar + inputFileName;
                    var outputFilePath = exePath + Path.DirectorySeparatorChar + outputFileName + DateTime.Now.Minute + DateTime.Now.Second + "." + outputFileType;

                    CsvManager manager = new CsvManager(nodePrefix, spName, inputFilePath, outputFilePath);

                    Console.WriteLine("Reading from " + inputFilePath);
                    manager.Read();

                    Console.WriteLine("Generating script");
                    manager.GenerateScript();

                    Console.WriteLine("Finished generating script:" + outputFilePath + ". Press Enter to exit.");
                }
                else
                {
                    Console.WriteLine("Incorrect Arguements. Please enter NodePrefix SpName InputFileName OutputFileName OutputFileType(sql/txt).");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.Read();

        }
    }
}
