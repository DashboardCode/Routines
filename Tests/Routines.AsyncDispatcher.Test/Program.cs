using DashboardCode.Routines;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DashboardCode.RoutinesPromisesTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string inputPath = currentDirectory + @"\DashboardCode.Routines.Promises.exe.config";
            string outputPath = currentDirectory + @"\app.copy.config";

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            
                AsyncManager.Run(async () =>
                {
                    using (StreamReader SourceReader = File.OpenText(inputPath))
                    using (StreamWriter DestinationWriter = File.CreateText(outputPath))
                        await CopyFilesAsync(SourceReader, DestinationWriter);
                    //throw new Exception("aaa");
                    await Task.Delay(1 * 1000);

                });
            

            if (!File.Exists(outputPath))
                throw new Exception("Test failed");

            
        }

        private static async Task CopyFilesAsync(StreamReader Source, StreamWriter Destination)
        {
            char[] buffer = new char[0x1000];
            int numRead;
            while ((numRead = await Source.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await Destination.WriteAsync(buffer, 0, numRead);
            }
        }
    }
}