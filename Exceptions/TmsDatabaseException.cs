namespace TmsApi.Exceptions;

public sealed class TmsDatabaseException(string message) : Exception(message);
