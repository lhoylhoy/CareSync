namespace CareSync.Application.Common.Geographics;

public sealed record ProvinceDto(
    string Code,
    string Name,
    string Region
);

public sealed record CityDto(
    string Code,
    string Name,
    string ProvinceCode,
    string ProvinceName,
    string? ZipCode
);

public sealed record BarangayDto(
    string Code,
    string Name,
    string CityCode,
    string CityName,
    string ProvinceCode,
    string ProvinceName,
    string? ZipCode
);
