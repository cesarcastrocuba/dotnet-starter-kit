using FSH.Framework.Persistence;
using Microsoft.Extensions.Hosting;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace Architecture.Tests;

/// <summary>
/// Architecture tests verifying that <see cref="DatabasePrecreatorHostedService"/>
/// respects the BuildingBlocks conventions: correct namespace, sealed, implements IHostedService.
/// </summary>
public class PersistenceHostedServicesTests
{
    private static readonly System.Reflection.Assembly PersistenceAssembly =
        typeof(IConnectionStringValidator).Assembly;

    [Fact]
    public void DatabasePrecreatorHostedService_Should_BeInPersistenceAssembly()
    {
        // Assert — type must be discoverable from the Persistence BuildingBlock assembly
        var type = PersistenceAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == nameof(DatabasePrecreatorHostedService));

        type.ShouldNotBeNull(
            $"DatabasePrecreatorHostedService must exist in assembly '{PersistenceAssembly.GetName().Name}'");
    }

    [Fact]
    public void DatabasePrecreatorHostedService_Should_BeSealed()
    {
        var result = Types
            .InAssembly(PersistenceAssembly)
            .That()
            .HaveNameEndingWith("PrecreatorHostedService")
            .Should()
            .BeSealed()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            "DatabasePrecreatorHostedService must be sealed to prevent unintentional inheritance.");
    }

    [Fact]
    public void DatabasePrecreatorHostedService_Should_ImplementIHostedService()
    {
        var type = PersistenceAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == nameof(DatabasePrecreatorHostedService));

        type.ShouldNotBeNull();
        type!.GetInterfaces()
            .ShouldContain(
                typeof(IHostedService),
                "DatabasePrecreatorHostedService must implement IHostedService.");
    }

    [Fact]
    public void DatabasePrecreatorHostedService_Should_BeInCorrectNamespace()
    {
        var type = PersistenceAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == nameof(DatabasePrecreatorHostedService));

        type.ShouldNotBeNull();
        type!.Namespace.ShouldBe(
            "FSH.Framework.Persistence",
            "DatabasePrecreatorHostedService must be in the FSH.Framework.Persistence namespace.");
    }

    [Fact]
    public void PersistenceHostedServices_Should_NotDependOnModules()
    {
        // Verifies the whole Persistence assembly doesn't depend on application modules
        var result = Types
            .InAssembly(PersistenceAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "FSH.Modules.Auditing",
                "FSH.Modules.Identity",
                "FSH.Modules.Multitenancy")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(
            "Persistence BuildingBlock must not depend on any application module.");
    }
}
