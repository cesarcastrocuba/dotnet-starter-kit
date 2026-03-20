using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Identity.Contracts;
using FSH.Modules.Multitenancy.Contracts;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Tests to ensure that DateTime is not used for temporal properties.
/// We standardize on DateTimeOffset across the entire project for better time zone handling.
/// </summary>
public class TemporalTypeComplianceTests
{
    private static readonly Assembly[] AssembliesToScan =
    [
        // Contracts
        typeof(AuditingContractsMarker).Assembly,
        typeof(IdentityContractsMarker).Assembly,
        typeof(MultitenancyContractsMarker).Assembly,
        
        // Modules (Domain/Implementation)
        // We use known types to get these assemblies since they don't have markers
        Assembly.Load("FSH.Modules.Identity"),
        Assembly.Load("FSH.Modules.Multitenancy"),
        Assembly.Load("FSH.Modules.Auditing")
    ];

    [Fact]
    public void Domain_And_Contracts_Should_Not_Use_DateTime()
    {
        var failingProperties = new List<string>();

        foreach (var assembly in AssembliesToScan)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsPublic || t.IsNestedPublic || t.IsNestedFamily || t.IsNestedFamORAssem);

            foreach (var type in types)
            {
                // Skip some infrastructure/system types if necessary
                if (type.FullName?.StartsWith("System.") == true) continue;
                if (type.IsInterface) continue;

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        failingProperties.Add($"{type.FullName}.{prop.Name} ({prop.PropertyType.Name})");
                    }
                }
            }
        }

        failingProperties.ShouldBeEmpty(
            $"The following properties use DateTime instead of the standardized DateTimeOffset:\n" +
            string.Join("\n", failingProperties));
    }

    [Fact]
    public void Properties_Ending_With_Utc_Should_Follow_OnUtc_Naming_Convention()
    {
        var failingProperties = new List<string>();

        foreach (var assembly in AssembliesToScan)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsPublic || t.IsNestedPublic);

            foreach (var type in types)
            {
                if (type.IsInterface) continue;
                
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    // If the property has a temporal nature and ends with Utc but doesn't follow OnUtc
                    if (prop.Name.EndsWith("Utc") && !prop.Name.EndsWith("OnUtc"))
                    {
                        // Some exceptions might be needed for library-defined names if any, 
                        // but for our domain/contracts we want OnUtc.
                        failingProperties.Add($"{type.FullName}.{prop.Name}");
                    }
                }
            }
        }

        failingProperties.ShouldBeEmpty(
            $"The following properties should follow the 'OnUtc' naming convention (e.g. CreatedOnUtc instead of CreatedUtc):\n" +
            string.Join("\n", failingProperties));
    }
}
