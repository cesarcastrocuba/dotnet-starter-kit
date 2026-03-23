using FSH.Framework.Caching;
using FSH.Modules.Auditing;
using FSH.Modules.Identity;
using FSH.Modules.Multitenancy;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace Architecture.Tests;

public class CachingGuardrailTests
{
    [Fact]
    public void BusinessModules_ShouldNot_DependOn_ICacheService_Directly()
    {
        var modules = new[]
        {
            typeof(AuditingModule).Assembly,
            typeof(IdentityModule).Assembly,
            typeof(MultitenancyModule).Assembly
        };

        foreach (var module in modules)
        {
            var result = Types
                .InAssembly(module)
                .ShouldNot()
                .HaveDependencyOn("FSH.Framework.Caching.ICacheService")
                .GetResult();

            var failingTypes = result.FailingTypeNames ?? Array.Empty<string>();

            result.IsSuccessful.ShouldBeTrue(
                $"Business modules must not depend directly on ICacheService. Use ITenantCacheService instead. " +
                $"Failing types in {module.FullName}: {string.Join(", ", failingTypes)}");
        }
    }
}
