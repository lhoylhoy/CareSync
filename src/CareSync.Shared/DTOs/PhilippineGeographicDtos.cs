namespace CareSync.Shared.DTOs;

public record ProvinceDto(
    string Code,
    string Name,
    string Region
);

public record CityDto(
    string Code,
    string Name,
    string ProvinceCode,
    string ProvinceName,
    string? ZipCode
);

public record BarangayDto(
    string Code,
    string Name,
    string CityCode,
    string CityName,
    string ProvinceCode,
    string ProvinceName,
    string? ZipCode
);
