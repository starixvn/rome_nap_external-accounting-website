using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fanex.ExternalAccount.ReportsAndStatements.Services.Configuration
{
    public class StoredProcedureInfo
    {
        public string Key { get; set; }
        public string StoredProcedureName { get; set; }
        public string DatabaseName { get; set; }
        public bool IsPlain { get; set; }
        public int? Timeout { get; set; }
        public Dictionary<string, string> Mappings { get; set; } = new Dictionary<string, string>();
    }

    public class StoredProcedureService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StoredProcedureService> _logger;
        private readonly Dictionary<string, StoredProcedureInfo> _storedProcedures = new();

        public StoredProcedureService(IConfiguration configuration, ILogger<StoredProcedureService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            LoadStoredProcedures();
        }

        private void LoadStoredProcedures()
        {
            try
            {
                // Load the SpNames XML file
                var spNamesPath = _configuration["StoredProcedurePath"]?.Replace("~", ".") ?? "SpNames.CustDB.xml";

                if (!File.Exists(spNamesPath))
                {
                    _logger.LogWarning("Stored procedure XML file not found at path: {Path}", spNamesPath);
                    return;
                }

                var doc = XDocument.Load(spNamesPath);

                foreach (var function in doc.Descendants("Function"))
                {
                    var functionName = function.Attribute("Name")?.Value;

                    foreach (var setting in function.Descendants("Setting"))
                    {
                        var key = setting.Attribute("Key")?.Value;
                        var spName = setting.Attribute("SpName")?.Value;
                        var dbName = setting.Attribute("DbName")?.Value;
                        var plain = setting.Attribute("Plain")?.Value == "1";
                        var timeoutStr = setting.Attribute("TimeOut")?.Value;

                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(spName) || string.IsNullOrEmpty(dbName))
                            continue;

                        var spInfo = new StoredProcedureInfo
                        {
                            Key = key,
                            StoredProcedureName = spName,
                            DatabaseName = dbName,
                            IsPlain = plain,
                            Timeout = string.IsNullOrEmpty(timeoutStr) ? null : int.Parse(timeoutStr)
                        };

                        // Load field mappings
                        foreach (var mapping in setting.Descendants("Mapping"))
                        {
                            var dataField = mapping.Attribute("DataField")?.Value;
                            var objectProperty = mapping.Attribute("ObjectProperty")?.Value;

                            if (!string.IsNullOrEmpty(dataField) && !string.IsNullOrEmpty(objectProperty))
                            {
                                spInfo.Mappings[dataField] = objectProperty;
                            }
                        }

                        _storedProcedures[key] = spInfo;
                    }
                }

                _logger.LogInformation("Loaded {Count} stored procedure mappings", _storedProcedures.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stored procedure configurations");
            }
        }

        public StoredProcedureInfo GetStoredProcedure(string key)
        {
            return _storedProcedures.TryGetValue(key, out var sp) ? sp : null;
        }

        public string GetConnectionString(string databaseName)
        {
            return _configuration.GetConnectionString(databaseName);
        }

        public IEnumerable<StoredProcedureInfo> GetAllStoredProcedures()
        {
            return _storedProcedures.Values;
        }

        public IEnumerable<StoredProcedureInfo> GetStoredProceduresByFunction(string functionName)
        {
            return _storedProcedures.Values.Where(sp => sp.Key.StartsWith(functionName));
        }
    }
}
