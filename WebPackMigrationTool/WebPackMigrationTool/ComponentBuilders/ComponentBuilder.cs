using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebPackMigrationTool.Components;

namespace WebPackMigrationTool.ComponentBuilders
{
    public abstract class ComponentBuilder
    {
        protected static string RegexComment = @"(\/\*[\s\S]*\*\/\s*)*";
        protected List<IComponent> components = new List<IComponent>();

        public List<IComponent> Components
        {
            get { return components; }
        }

        public string GetFormatedString(string filePath, string fileContentText, string fileName,
                                         string partialPath, ComponentType componentType, string regexComponent)
        {
            string formatedString = string.Empty;
            var regexMatches = Regex.Matches(fileContentText, regexComponent);

            foreach (Match regexMatch in regexMatches)
            {
                var componentComments = regexMatch.Groups[1].ToString();
                string componentFriendlyName = GetFriendlyName(regexMatch.Groups[3].ToString());

                formatedString += string.Format(
                    "{0}{1}.$inject =\n[{2}];\nfunction {3}{4}\nmodule.exports = {1};",
                    componentComments,
                    componentFriendlyName,
                    regexMatch.Groups[4].ToString(),
                    componentFriendlyName,
                    regexMatch.Groups[5].ToString());

                var componentName = regexMatch.Groups[3].ToString();
                var parent = regexMatch.Groups[2].ToString();
                components.Add(new Component(componentName, componentFriendlyName, filePath, fileName,
                                                  partialPath, parent, componentType));
            }
            return formatedString;
        }

        public string GetFriendlyName(string originalName)
        {
            var splittedName = originalName.Split('.');
            return splittedName[splittedName.Length - 1];
        }

        public bool IsMatch(string fileContent)
        {
            return Regex.IsMatch(fileContent, RegexComponent);
        }

        public abstract string RegexComponent { get; }

        public abstract string GetFormatedString(string filePath, string fileContentText, string fileName,
            string partialPath);
    }
}
