using System.Collections.Concurrent;

namespace CareSync.Web.Admin.Common.Filtering;

public sealed class FilterDefinition
{
    private readonly List<FilterOption> _options;
    public enum FilterKind
    {
        Select,
        Date
    }

    public FilterDefinition(
        string key,
        string label,
        IEnumerable<FilterOption> options,
        string? propertyName = null,
        bool autoPopulate = false,
        FilterKind kind = FilterKind.Select)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Filter key cannot be null or whitespace.", nameof(key));

        Key = key;
        Label = string.IsNullOrWhiteSpace(label) ? key : label;
        PropertyName = string.IsNullOrWhiteSpace(propertyName) ? key : propertyName;
        AutoPopulate = autoPopulate;
        Kind = kind;
        _options = options?.ToList() ?? new List<FilterOption>();
    }

    public string Key { get; }
    public string Label { get; }
    public string PropertyName { get; }
    public bool AutoPopulate { get; }
    public FilterKind Kind { get; }

    public IReadOnlyList<FilterOption> Options => _options;

    public FilterDefinition Clone()
        => new(Key, Label, _options, PropertyName, AutoPopulate);

    public void ReplaceOptions(IEnumerable<FilterOption> options)
    {
        _options.Clear();
        if (options == null) return;
        _options.AddRange(options);
    }
}

public sealed record FilterOption(string Value, string Label);

public sealed class FilterChangedEventArgs : EventArgs
{
    public FilterChangedEventArgs(string key, string? value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public string? Value { get; }
}
