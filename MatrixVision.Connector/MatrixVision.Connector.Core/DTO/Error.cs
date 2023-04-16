namespace MatrixVision.Connector.Core
{
    public class Error
    {
        public string Message { get; } = "";
        public string Type { get; set; } = "";

        public Error(string message, string type)
        {
            Message = message;
            Type = type;
        }
    }
}