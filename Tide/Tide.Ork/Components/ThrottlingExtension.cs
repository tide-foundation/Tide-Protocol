using System;
using Tide.Ork.Classes;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ThrottlingExtension
    {
        public static void ConfigureThrottling(this IServiceCollection services)
        {
            services.AddSingleton(new BlackList());
            services.AddSingleton(new ThrottlingManager());
        }

        public static void ConfigureThrottling(this IServiceCollection services, ushort allow, TimeSpan lapse, TimeSpan blockFor)
        {
            services.AddSingleton(new BlackList(blockFor));
            services.AddSingleton(new ThrottlingManager()
            {
                Allow = allow,
                Lapse = lapse
            });
        }
    }
}