namespace Console
{
    using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPackMigrationTool;

    public class Program
    {
        public static void Main(string[] args)
        {
            string SourcePath = ConfigurationManager.AppSettings["sourcePath"];
            string DestinationPath = ConfigurationManager.AppSettings["destinationPath"];

            MigrationTool migrationTool = new MigrationTool(SourcePath, DestinationPath);
            try
            {
                migrationTool.Start();
            }
            catch (DirectoryNotFoundException directoryException)
            {
                Console.WriteLine(directoryException.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}
