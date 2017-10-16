using Core.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Data.Cache
{
    public class CacheTimer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheTimer> _logger;
        private readonly IApplicationConfig _applicationConfig;
        private static readonly object _lockObject = new object();

        private Timer _timer;

        private AutoResetEvent _autoResetEvent;

        public CacheTimer(
            IServiceProvider serviceProvider,
            ILogger<CacheTimer> logger,
            IApplicationConfig applicationConfig)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _applicationConfig = applicationConfig;
            StartTimer();
        }

        public void StartTimer()
        {
            _autoResetEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoResetEvent, TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(_applicationConfig.Intervals.CacheTimerSeconds));
        }

        public void Execute(object stateInfo)
        {
            lock (_lockObject)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var services = scope.ServiceProvider;
                        var cacheInitializer = services.GetRequiredService<IProductModelCacheManager>();
                        cacheInitializer.UpdateCache();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while updating cache.");
                    }
                }
            }
        }
    }
}
