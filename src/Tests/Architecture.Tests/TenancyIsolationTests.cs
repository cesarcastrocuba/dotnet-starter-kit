using FSH.Framework.Core.Domain;
using NetArchTest.Rules;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Tests to ensure that all entities implementing IHasTenant are correctly
/// configured with multi-tenancy isolation in EF Core.
/// </summary>
public class TenancyIsolationTests
{
    private static readonly string SolutionRoot = ModuleArchitectureTestsFixture.SolutionRoot;

    [Fact]
    public void Entities_Implementing_IHasTenant_Should_Have_IsMultiTenant_Configuration()
    {
        // 1. Explicitly load all relevant assemblies
        var assemblies = new List<Assembly>
        {
            typeof(IHasTenant).Assembly,                                    // Core
            Assembly.Load("FSH.Framework.Eventing"),                        // Eventing (Outbox/Inbox)
            Assembly.Load("FSH.Modules.Multitenancy"),                      // Multitenancy (Theme/Provisioning)
            Assembly.Load("FSH.Modules.Identity"),                          // Identity
            Assembly.Load("FSH.Modules.Auditing")                           // Auditing
        };

        // 2. Find all types implementing IHasTenant in these assemblies
        var tenantEntities = Types.InAssemblies(assemblies)
            .That()
            .ImplementInterface(typeof(IHasTenant))
            .And()
            .AreNotInterfaces()
            .And()
            .AreNotAbstract()
            .GetTypes();

        tenantEntities.ShouldNotBeEmpty("No entities implementing IHasTenant were found.");

        var failures = new List<string>();

        foreach (var entityType in tenantEntities)
        {
            // Skip entities that are handled by MultiTenantIdentityDbContext automatically
            if (entityType.Name == "FshUser" || entityType.Name == "FshRole" || 
                entityType.Name.StartsWith("IdentityUser", StringComparison.Ordinal) || entityType.Name.StartsWith("IdentityRole", StringComparison.Ordinal))
            {
                continue;
            }

            string entityName = entityType.Name;
            string assemblyName = entityType.Assembly.GetName().Name!;

            // 2. Find the configuration file
            // Pattern: {EntityName}Configuration.cs
            // We search in the same project/module
            
            // More robust: search all files in src matching {entityName}Configuration.cs
            string searchPattern = $"{entityName}Configuration.cs";
            var configFiles = Directory.GetFiles(Path.Combine(SolutionRoot, "src"), searchPattern, SearchOption.AllDirectories);

            if (configFiles.Length == 0)
            {
                failures.Add($"{entityName} ({assemblyName}): No configuration file found matching '{searchPattern}'.");
                continue;
            }

            bool isConfiguredCorrectly = false;
            foreach (var configFile in configFiles)
            {
                string content = File.ReadAllText(configFile);
                if (content.Contains(".IsMultiTenant("))
                {
                    isConfiguredCorrectly = true;
                    break;
                }
            }

            if (!isConfiguredCorrectly)
            {
                failures.Add($"{entityName} ({assemblyName}): Configuration found but missing '.IsMultiTenant()' call.");
            }
        }

        failures.ShouldBeEmpty(
            $"The following IHasTenant entities are not correctly configured for multi-tenancy isolation:\n" +
            string.Join("\n", failures));
    }
}
