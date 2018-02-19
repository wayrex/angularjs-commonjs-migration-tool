using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class ControllerComponentBuilder : ComponentBuilder
    {
        private readonly string _regexController =
            @"angular\s*\.module\([""'](.[^\(\)\[\]\{\}]+)[""']\)\s*\.controller\([""']([^\(\)\[\]\{\}]*)[""'],\s*\[([^\(\)\[\]\{\}]*)function\s*(\([\s\S]*)(\]\);)";
        public override string RegexComponent => RegexComment + _regexController;

        public override string GetFormatedString(string filePath, string fileContentText, 
                                                 string fileName, string partialPath)
        {
            return GetFormatedString(filePath, fileContentText, fileName, partialPath, 
                                     ComponentType.Controller, RegexComponent);
        }
    }
}
