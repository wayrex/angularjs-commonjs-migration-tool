using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class ServiceComponentBuilder : ComponentBuilder
    {
        private readonly string _regexService =
            @"angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*\.service\([""']([^\(\)\[\]\{\}]*)[""'],\s*\[([^\(\)\[\]\{\}]*)function(\s*\([\s\S]*)(\]\);)";
        public override string RegexComponent => RegexComment + _regexService;
        
        public override string GetFormatedString(string filePath, string fileContentText, string fileName, string partialPath)
        {
            return GetFormatedString(filePath, fileContentText, fileName, partialPath,
                                     ComponentType.Service, RegexComponent);
        }
    }
}
