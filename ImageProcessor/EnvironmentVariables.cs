using System;

namespace ImageProcessor
{
    public static class EnvironmentVariables
    {
        static Uri _documentDbUri;

        public static readonly string DocumentDbUrl = Environment.GetEnvironmentVariable(RemoteDocumentDbUrl);
        public static readonly string DocumentDbKey = Environment.GetEnvironmentVariable(RemoteDocumentDbKey);
        public static readonly string StorageAccountConnection = Environment.GetEnvironmentVariable(AzureWebJobsStorage);
        public static readonly string ComputerVisionApiKey = Environment.GetEnvironmentVariable(CognitiveServicesKey);

        public const string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
        public const string CognitiveServicesKey = nameof(CognitiveServicesKey);
        public const string RemoteDocumentDbUrl = nameof(RemoteDocumentDbUrl);
        public const string RemoteDocumentDbKey = nameof(RemoteDocumentDbKey);


        public static readonly string VisionUrl = "https://eastus.api.cognitive.microsoft.com/vision/v1.0";
    }
}
