using Oracle.ManagedDataAccess.Client;
using Simego.DataSync.Providers.Ado;
using System;
using System.Data;
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

        protected override void FillRegistryView(RegistryFolderType rootFolder, RegistryFolder tables, RegistryFolder views)
        {
            var factory = GetProviderFactory(ProviderName);

            using (var connection = factory.GetConnection(ConnectionString.GetConnectionString()))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command = factory.CreateCommand($"SELECT ALL_OBJECTS.OWNER, ALL_OBJECTS.OBJECT_NAME, USER_OBJECTS.OBJECT_TYPE FROM ALL_OBJECTS INNER JOIN USER_OBJECTS ON USER_OBJECTS.OBJECT_ID = ALL_OBJECTS.OBJECT_ID WHERE USER_OBJECTS.OBJECT_TYPE = 'TABLE' OR USER_OBJECTS.OBJECT_TYPE = 'VIEW'", connection))
                {
                    command.CommandTimeout = CommandTimeout;
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schema = reader.GetString(0);
                            var table = reader.GetString(1);
                            var type = reader.GetString(2);

                            var isDbSchema = schema.Equals(connection.Database, StringComparison.OrdinalIgnoreCase);
                            var name = isDbSchema ? $"{table}" : $"{schema}.{table}";
                            var qualifiedName = isDbSchema ? $"{StartQuoteChar}{table}{EndQuoteChar}" : $"{StartQuoteChar}{schema}{EndQuoteChar}.{StartQuoteChar}{table}{EndQuoteChar}";

                            if (string.Equals(type, "VIEW", StringComparison.OrdinalIgnoreCase))
                            {
                                views.AddFolderItem(qualifiedName, name);
                            }
                            else
                            {
                                tables.AddFolderItem(qualifiedName, name);
                            }
                        }
                    }
                }
            }
        }
    }
}
