using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebPackMigrationTool.Components;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class RunComponentBuilder : ComponentBuilder
    {
        private readonly string _regexRun =
           @"angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*\.run\s*\(\s*\[(\s*[""']([^\(\)\[\]\{\}]*)[""']),\s*function(\s*\([\s\S]*)(\]\);)";
        public override string RegexComponent => RegexComment + _regexRun;

        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            string formatedString = string.Empty;
            var regexMatches = Regex.Matches(fileContentText, RegexComponent);

            foreach (Match regexMatch in regexMatches)
            {
                var componentComments = regexMatch.Groups[1].ToString();
                string componentFriendlyName = $"{regexMatch.Groups[2].ToString()}{ComponentType.Run.ToString()}";

                formatedString += string.Format(
                    "{0}{1}.$inject =\n[{2}];\nfunction {3}{4}\nmodule.exports = {1};",
                    componentComments,
                    componentFriendlyName,
                    regexMatch.Groups[3].ToString(),
                    componentFriendlyName,
                    regexMatch.Groups[5].ToString());

                var componentName = componentFriendlyName;
                var parent = regexMatch.Groups[2].ToString();
                components.Add(new Component(componentName, 
                                             componentFriendlyName, 
                                             filePath, 
                                             fileName,
                                             partialPath, 
                                             parent, 
                                             ComponentType.Run));
            }
            return formatedString;
        }
    }
}
