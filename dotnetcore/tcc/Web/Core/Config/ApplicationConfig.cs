using Core.Config;
using Microsoft.Extensions.Options;

namespace Web.Core.Config
{
    public class ApplicationConfig : IApplicationConfig
    {
        public ApplicationConfig(
            IOptions<ConnectionStrings> optConnectionStrings,
            IOptions<Intervals> optIntervals)
        {
            ConnectionStrings = optConnectionStrings.Value;
            Intervals = optIntervals.Value;
        }

        public ConnectionStrings ConnectionStrings { get; }

        public Intervals Intervals { get; }
    }
}
