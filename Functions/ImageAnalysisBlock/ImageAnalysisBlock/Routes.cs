using System;
namespace ImageAnalysisBlock
{
    public static class Routes
    {
        public const string Get = "get";

        public const string Post = "post";

        public const string Json = "application/json";

        public const string BaseBlobUriRoute = "https://ImageAnalysisBlockmiwats.blob.core.windows.net/";

        public const string OriginalImageBlob = "original-images/{id}";
        public const string CogsImageBlob = "images-for-cogs/{id}";
        public const string OcrDrawImageBlob = "ocr-draw/{id}";
        public const string ThumbnailBlob = "thumbnails/{id}";


        public const string GenericAnalysisQueue = "generic-analysis";
        public const string OcrAnalysisQueue = "ocr-analysis";
        public const string OcrDrawTextQueue = "ocr-draw";
        public const string HandwrittenAnalysisQueue = "handwritten-analysis";
        public const string CelebLandmarkAnalysisQueue = "celeb-landmark-analysis";
        public const string ThumbnailQueue = "thumbnail";
        public const string DataSaveQueue = "data-save";


        public const string Metadata = "metadata";
        public const string AnalyzedImages = "analyzed-images";

        public const string LoginGoogle = ".auth/login/google?access_type=offline";

        public const string AuthenticateUser = "api/user/config";

        public const string EncodeBlob = "uploads-avcontent/{fileName}.{fileExtension}";

        public const string GenerateContentToken = "api/tokens/content/{collectionId}";

        public static string ContentToken(string collectionId) => $"api/tokens/content/{collectionId}";

        public const string GenerateStorageToken = "api/tokens/storage/{collectionId}/{documentId}";

        public static string StorageToken(string collectionId, string documentId) => $"api/tokens/storage/{collectionId}/{documentId}";

        public const string NotifyClients = "";

        public const string PublishContent = "api/publish";

        public const string UpdateAvContent = "";

        public const string GetAppSettings = "api/settings";
    }
}