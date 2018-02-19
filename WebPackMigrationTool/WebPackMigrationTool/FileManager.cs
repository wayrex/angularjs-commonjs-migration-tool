namespace WebPackMigrationTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using WebPackMigrationTool.Components;

    public class FileManager
    {
        private FileProcessor fileProcessor = new FileProcessor();

        public void ProcessFolder(string path = "E:/Internship/Migration/pros-ps-commonclient/src")
        {
            if (Directory.Exists(path))
            {
                this.ProcessDirectory(path);
                fileProcessor.ProcessModulesDependencies();
                fileProcessor.ProcessComponents();
            }
            else
            {
                throw new DirectoryNotFoundException(
                    string.Format("{0} is not a valid file or directory.", path));
            }
        }

        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            else
            {
                this.DeleteContent(new DirectoryInfo(destDirName));
            }

            FileInfo[] files = directoryInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in directories)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    this.DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                this.fileProcessor.ProcessFile(fileName, targetDirectory);
            }

            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                this.ProcessDirectory(subdirectory);
            }
        }

        private void DeleteContent(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
            }
        }
    }
}
