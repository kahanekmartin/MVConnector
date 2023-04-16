using System.Collections.Generic;

namespace MatrixVision.Connector.Core
{
    public class Response
    {
        public bool Success { get; set; }
        public List<Asset> Images { get; set; } = new();
        public List<Error> Errors { get; set; } = new();

        public static Response Error(string errorMessage, string type = "") => new()
        {
            Success = false,
            Errors = new List<Error> { new Error(errorMessage, type) }
        };

        public static Response CapturedSingle(Asset image) => new()
        {
            Success = true,
            Images = new List<Asset> { image }
        };
    }
}