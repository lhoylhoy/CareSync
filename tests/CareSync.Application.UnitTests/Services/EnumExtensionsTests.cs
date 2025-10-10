using CareSync.Domain.Enums;
using CareSync.Domain.Extensions;
using Xunit;

namespace CareSync.Application.UnitTests.Services;

public class EnumExtensionsTests
{
    [Fact]
    public void GetDisplayName_Returns_DisplayAttribute_WhenPresent()
    {
        var display = StaffRole.PharmacyTechnician.GetDisplayName();
        Assert.Equal("Pharmacy Technician", display);
    }

    [Fact]
    public void GetDisplayName_FallsBack_ToSplitPascalCase()
    {
        var display = StaffRole.ItSupport.GetDisplayName();
        Assert.Equal("IT Support", display);
    }
}
