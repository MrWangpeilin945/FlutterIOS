namespace Ryobi.Wellship.WebAPI.ExternalConnection.Model.Standard
{
    public class ErrorObject
    {
        public required string Code { get; init; }

        public required string Message { get; init; }
    }
}
