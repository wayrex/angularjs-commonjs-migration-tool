using WebPackMigrationTool.ComponentBuilders;

namespace WebPackMigrationTool
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using WebPackMigrationTool.Components;

    public class FileProcessor
    {
        private static readonly Char delimiter = '|';
        private static readonly string[] StyleFormats = ConfigurationManager.AppSettings["styleFormats"].Split(delimiter);
        private static readonly string[] ExcludedFiles = ConfigurationManager.AppSettings["excludedFiles"].Split(delimiter);

        private Dictionary<string, IComponent> moduleDictionary;
        private List<IComponent> components;
        private List<string> stylesheetPaths;

        public FileProcessor()
        {
            moduleDictionary = new Dictionary<string, IComponent>();
            components = new List<IComponent>();
            stylesheetPaths = new List<string>();

        }

        public void ProcessFile(string filePath, string targetDirectory)
        {
            var partialPath = filePath.Split(new string[] { targetDirectory }, StringSplitOptions.None)[1];
            if (!this.IsValidFile(partialPath))
            {
                return;
            }

            var fileContent = File.ReadLines(filePath).ToList();
            fileContent = this.RemoveIife(fileContent);
            string fileName = Path.GetFileName(filePath);

            string fileContentText = string.Join("\r\n", fileContent);

            if (IsStylesheet(fileName))
            {
                stylesheetPaths.Add(filePath);
            }

            var componentBuilderDirector = new ComponentBuilderDirector();
            ComponentBuilder componentBuilder = componentBuilderDirector.CreateComponentBuilder(fileContentText);

            string formatedString;

            if (componentBuilder != null)
            {
                formatedString = componentBuilder.GetFormatedString(filePath, fileContentText, fileName, partialPath);

                components.AddRange(componentBuilder.Components);
            }
            else
            {
                formatedString = fileContentText;
            }

            File.WriteAllText(filePath, formatedString);
        }

        public void InyectStylesheets()
        {
            foreach (string stylesheetPath in stylesheetPaths)
            {
                this.InyectStylesheet(stylesheetPath);
            }
        }

        public void InyectStylesheet(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);
            string targetDirectory = Directory.GetParent(fullPath).ToString();

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            if (fileEntries.Any(file => StylesheetHasComponent(fileName, file, "module")))
            {
                return;
            }
            if (fileEntries.Any(file => StylesheetHasComponent(fileName, file, "directive")))
            {
                return;
            }
        }

        public List<string> RemoveIife(List<string> fileContent)
        {
            const string useStrictSingle = @"'use strict';";
            const string useStrictDouble = @"""use strict"";";

            for (int i = 0; i < fileContent.Count(); i++)
            {
                if (fileContent[i].Contains(useStrictSingle) || fileContent[i].Contains(useStrictDouble))
                {
                    fileContent[i] = fileContent[i].Replace(useStrictSingle, string.Empty);
                    fileContent[i] = fileContent[i].Replace(useStrictDouble, string.Empty);

                    if (string.IsNullOrEmpty(fileContent[i].Trim()))
                    {
                        fileContent.RemoveAt(i);
                        i--;
                    }
                }
            }
            return fileContent;
        }

        public MatchCollection RegexFind(string fileContent, string regexPattern)
        {
            if (Regex.IsMatch(fileContent, regexPattern))
            {
                return Regex.Matches(fileContent, regexPattern);
            }

            return null;
        }

        public string GetFriendlyName(string originalName)
        {
            var splittedName = originalName.Split('.');
            return splittedName[splittedName.Length - 1];
        }

        public bool IsStylesheet(string filename)
        {
            string fileExtension = Path.GetExtension(filename);
            if (StyleFormats.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        public void ProcessComponents()
        {
            foreach (var component in this.components)
            {
                if (!ComponentType.Module.Equals(component.ComponentType) && moduleDictionary.ContainsKey(component.Parent))
                {
                    var module = moduleDictionary[component.Parent];
                    string fileContent = File.ReadAllText(module.Path);

                    string requirePath = GetRequirePathString(component.Path, module.Path);

                    var splittedName = component.Parent.Split('.');
                    string friendlyName = splittedName[splittedName.Length - 1];

                    string newComponent =
                        $"{friendlyName}.{component.ComponentType.ToString().ToLower()}('{component.Name}'," +
                        $" require('{requirePath}'));\n/*{component.Parent}-MIGRATION COMPONENTS*/";
                    string result = fileContent.Replace($"/*{component.Parent}-MIGRATION COMPONENTS*/", newComponent);
                    File.WriteAllText(module.Path, result);
                }
            }
            this.InyectStylesheets();
        }

        public void ProcessModulesDependencies()
        {
            var delimiter = ',';
            string regexModuleDependencies = @"((angular\s*\.module\('[^']*',\s*\[)([^\]]*)(\]))+";

            FillModuleDictionary();

            foreach (var module in moduleDictionary)
            {
                string modulePath = module.Value.Path;
                string fileContent = File.ReadAllText(modulePath);
                var dependenciesMatches = Regex.Matches(fileContent, regexModuleDependencies);
                foreach (Match dependenciesMatch in dependenciesMatches)
                {
                    var cleanDependencyMatch = dependenciesMatch.Groups[3].ToString()
                                                                .Replace(System.Environment.NewLine, string.Empty);
                    var dependencies = cleanDependencyMatch.Split(delimiter);
                    foreach (var dependency in dependencies)
                    {
                        string cleanDependency = dependency.Replace("'", string.Empty).Replace(" ", string.Empty);
                        if (moduleDictionary.ContainsKey(cleanDependency))
                        {
                            string dependencyPath = moduleDictionary[cleanDependency].Path;
                            string relativePath = GetRequirePathString(dependencyPath, modulePath);
                            fileContent =
                                $"var {GetFriendlyName(cleanDependency)} = require('{relativePath}');\n{fileContent}";
                        }
                    }
                }
                File.WriteAllText(module.Value.Path, fileContent);
            }
        }

        private void FillModuleDictionary()
        {
            var moduleComponents = components.Where(module => module.ComponentType.Equals(ComponentType.Module));
            foreach (var module in moduleComponents)
            {
                if (!moduleDictionary.ContainsKey(module.Name))
                {
                    moduleDictionary.Add(module.Name, module);
                }
            }
        }

        private string GetRequirePathString(string requiredPath, string requireePath)
        {
            Uri requiredPathUri = new Uri(requiredPath);
            Uri requireePathUri = new Uri(requireePath);
            Uri diff = requireePathUri.MakeRelativeUri(requiredPathUri);
            return diff.ToString();
        }

        private bool StylesheetHasComponent(string stylesheetFileName, string filePath, string component)
        {
            string componentFileName = Path.GetFileName(filePath);
            Char delimiter = '.';
            if (componentFileName != null)
            {
                var splittedFileName = componentFileName.Split(delimiter);
                var splittedStylesheetFileName = stylesheetFileName.Split(delimiter);
                if (splittedFileName.Contains(component))
                {
                    if (splittedFileName[0] == splittedStylesheetFileName[0])
                    {
                        string fileContent = File.ReadAllText(filePath);
                        fileContent = $"require('{stylesheetFileName}');\n{fileContent}";
                        File.WriteAllText(filePath, fileContent);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsValidFile(string fileName)
        {
            if (!HasValidExtension(fileName))
            {
                return false;
            }
            if (IsExcludedFile(fileName))
            {
                return false;
            }
            return true;
        }

        private bool IsExcludedFile(string fileName)
        {
            if (ExcludedFiles.Contains(fileName))
            {
                return true;
            }
            return false;
        }

        private bool HasValidExtension(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            if (fileExtension == ".js")
            {
                return true;
            }
            if (StyleFormats.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

    }
}
