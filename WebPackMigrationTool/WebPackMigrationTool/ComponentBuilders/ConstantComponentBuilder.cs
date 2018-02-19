using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebPackMigrationTool.Components;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class ConstantComponentBuilder : ComponentBuilder
    {
        private const string ConstantComponentPattern =
            @"(angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*(\" +
            @".constant\([""'][^\(\)\[\]\{\}]*[""'],\s*[\s\S]*\))+;)";
        private const string BeforeConstantComponentPattern = @"(\(function\s*\(\)\s*{)?\s*([.|\n|\W|\w]*)";
        // Known bug when there's a ')' in commented code
        private const string ConstPattern = @"\s*\.constant\([""']([^\(\)\[\]\{\}]*)[""'],\s*([^\)]*)\)";
        public override string RegexComponent => RegexComment + ConstantComponentPattern;

        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            string formatedString = string.Empty;

            var regexMatches = Regex.Matches(fileContentText, $"{BeforeConstantComponentPattern}{RegexComponent}");
            
            foreach (Match regexMatch in regexMatches)
            {
                var parent = regexMatch.Groups[5].ToString();
                var constantMatches = Regex.Matches(regexMatch.Groups[4].ToString(), ConstPattern);
                formatedString += regexMatch.Groups[3].ToString();

                foreach (Match constantMatch in constantMatches)
                {
                    string friendlyName = GetFriendlyName(constantMatch.Groups[1].ToString());

                    string filterFriendlyName = this.GetFriendlyName(constantMatch.Groups[1].ToString());
                    var filterName = constantMatch.Groups[1].ToString();
                    if (!formatedString.Contains($"var {friendlyName}"))
                    {
                        formatedString += $"var {friendlyName} = {constantMatch.Groups[2].ToString()};";
                    }

                    formatedString += $"\nmodule.exports = {friendlyName};";

                    components.Add(new Component(filterName, filterFriendlyName, filePath, fileName, 
                                                      partialPath, parent, ComponentType.Constant));
                }
            }

            return formatedString;
        }
    }
}
