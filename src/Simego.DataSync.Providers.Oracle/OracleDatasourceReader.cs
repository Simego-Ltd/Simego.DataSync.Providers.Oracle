using Oracle.ManagedDataAccess.Client;
using Simego.DataSync.Providers.Ado;
using System;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace Simego.DataSync.Providers.Oracle
{
    [ProviderInfo(Name = "Oracle Managed Data Access (21.13)", Description = "Oracle .NET Framework Managed Data Access", Group = "SQL")]
    public class OracleDatasourceReader : AdoDataSourceReader
    {
        private Lazy<AdoDbProviderFactory> MyFactory => new Lazy<AdoDbProviderFactory>(() => new AdoDbProviderFactory("Oracle.ManagedDataAccess.Client", GetFactory()));

        protected override bool IsCustomAdoProvider() => true;

        public override AdoDbProviderFactory GetProviderFactory(string providerInvariantName) => MyFactory.Value;

        private static DbProviderFactory GetFactory()
        {
            var factoryType = typeof(OracleClientFactory);

            return factoryType.InvokeMember(factoryType.Name,
               BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
               null, null, null, CultureInfo.CurrentCulture) as DbProviderFactory;
        }
    }
}
