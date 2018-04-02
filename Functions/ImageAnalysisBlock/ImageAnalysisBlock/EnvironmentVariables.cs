using System;

namespace ImageAnalysisBlock
{
    public static class EnvironmentVariables
    {
        static Uri _documentDbUri;

        public static readonly string DocumentDbUrl = Environment.GetEnvironmentVariable(RemoteDocumentDbUrl);
        public static readonly string DocumentDbKey = Environment.GetEnvironmentVariable(RemoteDocumentDbKey);
        public static readonly string StorageAccountConnection = Environment.GetEnvironmentVariable(AzureWebJobsStorage);
        public static readonly string ComputerVisionApiKey = Environment.GetEnvironmentVariable(CognitiveServicesKey);
        public static readonly string NotificationHubConnectionString = Environment.GetEnvironmentVariable(NotificationHubConnection);
        public static readonly string NotificationHubNameString = Environment.GetEnvironmentVariable(NotificationHubName);

        public const string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
        public const string CognitiveServicesKey = nameof(CognitiveServicesKey);
        public const string RemoteDocumentDbUrl = nameof(RemoteDocumentDbUrl);
        public const string RemoteDocumentDbKey = nameof(RemoteDocumentDbKey);
        public const string NotificationHubConnection = nameof(NotificationHubConnection);
        public const string NotificationHubName = nameof(NotificationHubName);


        public static readonly string VisionUrl = "https://eastus.api.cognitive.microsoft.com/vision/v1.0";
    }
}
