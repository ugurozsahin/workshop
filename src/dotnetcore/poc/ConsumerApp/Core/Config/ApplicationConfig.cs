using Core.Config;

namespace ConsumerApp.Core.Config
{
    public class ApplicationConfig : IApplicationConfig
    {
        public ApplicationConfig(ConnectionStrings connectionStrings)
        {
            ConnectionStrings = connectionStrings;
        }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
