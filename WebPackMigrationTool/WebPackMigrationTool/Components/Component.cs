namespace WebPackMigrationTool.Components
{
    public class Component : IComponent
    {
        public Component(string componentName, string componentFriendlyName, string path, 
                         string fileName, string pathFromRoot, string parent, ComponentType componentType)
        {
            Name = componentName;
            FriendlyName = componentFriendlyName;
            Path = path;
            FileName = fileName;
            PathFromRoot = pathFromRoot;
            Parent = parent;
            ComponentType = componentType;
        }

        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public string Path { get; set; }

        public string FileName { get; set; }

        public string PathFromRoot { get; set; }

        public string Parent { get; set; }

        public ComponentType ComponentType { get; set; }
    }
}
