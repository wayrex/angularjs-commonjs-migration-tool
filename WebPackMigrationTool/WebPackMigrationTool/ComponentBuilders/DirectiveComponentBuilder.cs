using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebPackMigrationTool.Components;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class DirectiveComponentBuilder : ComponentBuilder
    {
        private readonly string _regexDirective =
            @"angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*\.directive\([""']([^\(\)\[\]\{\}]*)[""'],\s*\[([^\(\)\[\]\{\}]*)function(\s*\([\s\S]*)(\]\);)";
        public override string RegexComponent => RegexComment + _regexDirective;

        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            string formatedString = string.Empty;
            string regexTemplateUrl = @"([\s\S]*)templateUrl[\s]*: ([^,]*\+)* [""'](([^""']*)*)[""'](,*[\s\S]*)";
            var regexMatches = Regex.Matches(fileContentText, RegexComponent);
            foreach (Match regexMatch in regexMatches)
            {
                var directiveComments = regexMatch.Groups[1].ToString();
                string directiveFriendlyName = this.GetFriendlyName(regexMatch.Groups[3].ToString());
                Match templateMatch = Regex.Match(regexMatch.Groups[5].ToString(), regexTemplateUrl);

                string directiveBody =
                    $"{templateMatch.Groups[1].ToString()}template: template{templateMatch.Groups[5].ToString()}";

                var templateFileName = Path.GetFileName(templateMatch.Groups[3].ToString());

                formatedString += string.Format(
                    "var template = require('{0}');\n{1}{2}.$inject =\n[{3}];\nfunction {4}{5}\nmodule.exports = {2};",
                    templateFileName,
                    directiveComments,
                    directiveFriendlyName,
                    regexMatch.Groups[4].ToString(),
                    directiveFriendlyName,
                    directiveBody);

                var directiveName = regexMatch.Groups[3].ToString();
                var parent = regexMatch.Groups[2].ToString();
                components.Add(new Component(directiveName, directiveFriendlyName, filePath, fileName, partialPath, 
                                             parent, ComponentType.Directive));
            }

            return formatedString;
        }
    }
}
