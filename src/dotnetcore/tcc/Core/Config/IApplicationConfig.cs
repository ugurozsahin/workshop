namespace Core.Config
{
    public interface IApplicationConfig
    {
        ConnectionStrings ConnectionStrings { get; }
        Intervals Intervals { get; }
    }
}
