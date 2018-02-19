using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebPackMigrationTool.Components;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class ModuleComponentBuilder : ComponentBuilder
    {
        private readonly string _regexModule =
            @"((angular\s*\.module)\([""'](.[^\(\)\[\]\{\}]+)[""'],\s*\[)([^\]]*)(\]\))(\.[\s\S]*)*";
        public override string RegexComponent => RegexComment + _regexModule;

        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            var formatedString = string.Empty;
            var regexMatches = Regex.Matches(fileContentText, RegexComponent);
            foreach (Match regexMatch in regexMatches)
            {
                var moduleName = regexMatch.Groups[4].ToString();
                var moduleComments = regexMatch.Groups[1].ToString();
                string moduleFriendlyName = this.GetFriendlyName(regexMatch.Groups[4].ToString());

                formatedString += string.Format(
                    "{0}{1}var {2} = {3};\n/*{4}-MIGRATION COMPONENTS*/\n\nmodule.exports = {2};\n",
                    moduleComments,
                    regexMatch.Groups[1].ToString(),
                    moduleFriendlyName,
                    regexMatch.Groups[0].ToString(),
                    regexMatch.Groups[4].ToString());
                components.Add(new Component(moduleName, moduleFriendlyName, filePath, fileName,
                                                      partialPath, null, ComponentType.Module));
            }
            return formatedString;
        }
    }
}
