namespace WebPackMigrationTool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MigrationTool
    {
        private string sourcePath = string.Empty;
        private string destinationPath = string.Empty;

        public MigrationTool(string sourcePath, string destinationPath)
        {
            this.sourcePath = sourcePath;
            this.destinationPath = destinationPath;
        }

        public void Start()
        {
            FileManager fileManager = new FileManager();
            fileManager.DirectoryCopy(this.sourcePath, this.destinationPath, true);

            fileManager.ProcessFolder(this.destinationPath);
        }
    }
}
