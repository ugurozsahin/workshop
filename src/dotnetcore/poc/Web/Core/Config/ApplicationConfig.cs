using Core.Config;
using Microsoft.Extensions.Options;

namespace Web.Core.Config
{
    public class ApplicationConfig : IApplicationConfig
    {
        public ApplicationConfig(IOptions<ConnectionStrings> connectionStrings)
        {
            ConnectionStrings = connectionStrings.Value;
        }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
