namespace CareSync.Web.Admin.Shared;

public static class UiMessages
{
    public static string DeleteConfirmation(string entity, string? name = null) =>
        name == null
            ? $"Are you sure you want to delete this {entity}? This action cannot be undone."
            : $"Are you sure you want to delete the {entity} '{name}'? This action cannot be undone.";

    public static string Created(string entity) => $"{entity} created successfully.";
    public static string Updated(string entity) => $"{entity} updated successfully.";
    public static string Deleted(string entity) => $"{entity} deleted successfully.";
}
