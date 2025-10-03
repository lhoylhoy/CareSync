namespace CareSync.Application.Common.Mapping;

public interface IEntityMapper<in TSource, out TDest>
{
    public TDest Map(TSource source);
}
