using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class FactoryComponentBuilder : ComponentBuilder
    {
        private readonly string _regexFactory =
            @"angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*\.factory\([""']([^\(\)\[\]\{\}]*)[""'],\s*\[([^\(\)\[\]\{\}]*)function(\s*\([\s\S]*)(\]\);)";
        public override string RegexComponent => RegexComment + _regexFactory;

        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            return GetFormatedString(filePath, fileContentText, fileName, partialPath,
                                     ComponentType.Factory, RegexComponent);
        }
    }
}
