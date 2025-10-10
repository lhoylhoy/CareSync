namespace CareSync.Application.Common;

public static class PagingDefaults
{
    // Compile-time constant so it can be used in default parameter values and initializers
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 25;
    public const int MaxPageSize = 200;
}
