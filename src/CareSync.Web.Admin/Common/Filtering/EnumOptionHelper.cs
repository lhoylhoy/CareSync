using CareSync.Domain.Extensions;
using System.Collections.Concurrent;

namespace CareSync.Web.Admin.Common.Filtering;

public static class EnumOptionHelper
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<FilterOption>> _cache = new();

    public static IReadOnlyList<FilterOption> GetOptions<TEnum>() where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        return _cache.GetOrAdd(type, _ =>
        {
            var list = Enum.GetValues<TEnum>()
                .Select(e => new FilterOption(e.ToString(), e.GetDisplayName()))
                .ToList();
            return list;
        });
    }
}
