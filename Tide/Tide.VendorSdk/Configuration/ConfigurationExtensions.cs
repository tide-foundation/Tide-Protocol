using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Classes.Storage;

namespace Tide.VendorSdk.Configuration
{
    public static class ConfigurationExtensions
    {
        public static ITideConfiguration UseSqlServerStorage(this ITideConfiguration configuration, string connectionString)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            TideConfiguration.Configuration.Database = new SqlDatabase(connectionString);
            return configuration;
        }

        public static ITideConfiguration SetEndpoint(this ITideConfiguration configuration, string endpoint)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));

            TideConfiguration.Configuration.Endpoint = endpoint;
            return configuration;
        }
    }
}
