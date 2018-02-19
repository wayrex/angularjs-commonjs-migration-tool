using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebPackMigrationTool.ComponentBuilders
{
    public class ComponentBuilderDirector
    {
        private readonly List<ComponentBuilder> componentBuilders;
        private readonly ModuleComponentBuilder moduleComponentBuilder;

        public ComponentBuilderDirector()
        {
            moduleComponentBuilder = new ModuleComponentBuilder();
            componentBuilders = new List<ComponentBuilder>();
            foreach (var type in
                Assembly.GetAssembly(typeof (ComponentBuilder)).GetTypes()
                    .Where(myType => !myType.IsInstanceOfType(typeof (ModuleComponentBuilder)) && myType.IsClass
                                     && !myType.IsAbstract && myType.IsSubclassOf(typeof (ComponentBuilder))))
            {
                componentBuilders.Add((ComponentBuilder) Activator.CreateInstance(type));
            }
        }

        public ComponentBuilder CreateComponentBuilder(string fileContentText)
        {
            ComponentBuilder componentBuilder = null;
            // Module definition
            if (moduleComponentBuilder.IsMatch(fileContentText))
            {
                componentBuilder = moduleComponentBuilder;
            }
            else
            {
                foreach (var component in componentBuilders)
                {
                    if (component.IsMatch(fileContentText))
                    {
                        return component;
                    }
                }
            }

            return componentBuilder;
        }
    }
}