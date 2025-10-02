namespace CareSync.Application.Common.Mapping;

public interface IEntityMapper<in TSource, out TDest>
{
    TDest Map(TSource source);
}
