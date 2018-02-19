namespace WebPackMigrationTool
{
    public interface IComponent
    {
        string Name { get; set; }

        string FriendlyName { get; set; }

        string Path { get; set; }

        string FileName { get; set; }

        string PathFromRoot { get; set; }

        string Parent { get; set; }
        ComponentType ComponentType { get; set; }
    }

    public enum ComponentType
    {
        Constant,
        Controller,
        Directive,
        Factory,
        Filter,
        Module,
        Service,
        Run
    }
}
