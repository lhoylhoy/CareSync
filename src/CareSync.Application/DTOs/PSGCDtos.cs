using System.Text.Json;
using System.Text.Json.Serialization;

namespace CareSync.Application.DTOs;

/// <summary>
///     Response wrapper for PSGC Cloud API
/// </summary>
public class PsgcResponse<T>
{
    [JsonPropertyName("data")] public List<T> Data { get; set; } = new();
}

/// <summary>
///     DTO for PSGC Cloud Province API response
/// </summary>
public class PsgcProvince
{
    [JsonPropertyName("code")] public string PsgcCode { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string ProvName { get; set; } = string.Empty;

    [JsonPropertyName("region")] public string RegName { get; set; } = string.Empty;

    // For backward compatibility, derive region code from name
    public string RegCode => GetRegionCode(RegName);

    private static string GetRegionCode(string regionName)
    {
        return regionName switch
        {
            "Region I (Ilocos Region)" => "010000000",
            "Region II (Cagayan Valley)" => "020000000",
            "Region III (Central Luzon)" => "030000000",
            "Region IV-A (CALABARZON)" => "040000000",
            "Region V (Bicol Region)" => "050000000",
            "Region VI (Western Visayas)" => "060000000",
            "Region VII (Central Visayas)" => "070000000",
            "Region VIII (Eastern Visayas)" => "080000000",
            "Region IX (Zamboanga Peninsula)" => "090000000",
            "Region X (Northern Mindanao)" => "100000000",
            "Region XI (Davao Region)" => "110000000",
            "Region XII (SOCCSKSARGEN)" => "120000000",
            "National Capital Region (NCR)" => "130000000",
            "Cordillera Administrative Region (CAR)" => "140000000",
            "Bangsamoro Autonomous Region In Muslim Mindanao (BARMM)" => "190000000",
            "Region XIII (Caraga)" => "160000000",
            "MIMAROPA Region" => "170000000",
            _ => "000000000"
        };
    }
}

/// <summary>
///     DTO for PSGC Cloud City/Municipality API response
/// </summary>
public class PsgcCity
{
    [JsonPropertyName("code")] public string PsgcCode { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string CityMunName { get; set; } = string.Empty;

    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

    [JsonPropertyName("district")] public string District { get; set; } = string.Empty;

    [JsonPropertyName("zip_code")] public string ZipCode { get; set; } = string.Empty;

    [JsonPropertyName("region")] public string RegName { get; set; } = string.Empty;

    [JsonPropertyName("province")] public string ProvName { get; set; } = string.Empty;

    // Derive province code from the city code (first 7 digits + 3 zeros)
    public string ProvCode => PsgcCode.Length >= 7 ? PsgcCode.Substring(0, 7) + "000" : string.Empty;

    public bool IsCity => Type == "City";
    public bool IsMunicipality => Type == "Mun";
}

/// <summary>
///     DTO for PSGC Cloud Barangay API response
/// </summary>
public class PsgcBarangay
{
    [JsonPropertyName("code")] public string PsgcCode { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string BrgyName { get; set; } = string.Empty;

    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("region")] public string RegName { get; set; } = string.Empty;

    [JsonPropertyName("province")] public string ProvName { get; set; } = string.Empty;

    [JsonPropertyName("city_municipality")]
    public string CityMunName { get; set; } = string.Empty;

    [JsonPropertyName("zip_code")] public string ZipCode { get; set; } = string.Empty;

    [JsonPropertyName("district")] public string District { get; set; } = string.Empty;

    // Derive codes from the barangay code
    public string CityMunCode => PsgcCode.Length >= 7 ? PsgcCode.Substring(0, 7) + "000" : string.Empty;
    public string ProvCode => PsgcCode.Length >= 4 ? PsgcCode.Substring(0, 4) + "0000000" : string.Empty;
}

/// <summary>
///     Custom JSON converter that can handle both string and boolean values
///     (kept for backward compatibility but may not be needed with PSGC Cloud API)
/// </summary>
public class StringOrBoolConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.True => "true",
            JsonTokenType.False => "false",
            JsonTokenType.Null => string.Empty,
            _ => string.Empty
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
