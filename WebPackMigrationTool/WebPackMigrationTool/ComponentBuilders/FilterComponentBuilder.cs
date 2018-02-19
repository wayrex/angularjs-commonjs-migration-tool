using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class FilterComponentBuilder : ComponentBuilder
    {
        private readonly string _regexFilter =
            @"angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*\.filter\([""']([^\(\)\[\]\{\}]*)[""'],\s*\[([^\(\)\[\]\{\}]*)function(\s*\([\s\S]*)(\]\);)";
        public override string RegexComponent => RegexComment + _regexFilter;

        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            return GetFormatedString(filePath, fileContentText, fileName, partialPath,
                                     ComponentType.Filter, RegexComponent);
        }
    }
}
