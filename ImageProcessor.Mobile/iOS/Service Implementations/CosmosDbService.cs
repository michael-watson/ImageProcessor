using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageProcessor.Common.Interfaces;
using ImageProcessor.Common.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace ImageProcessor.Mobile.iOS.ServiceImplementations
{
    public class CosmosDbService : ICosmosDbService
    {
        static readonly string DatabaseId = "metadata";
        static readonly string CollectionId = "analyzed-images";
        static readonly string CosmosUrl = "https://image-processing-miwats.documents.azure.com:443/";
        static readonly string CosmosDbKey = "anlvPs5x0UuzHBptPgANpN6yKOnsLwcD3IwTcyvgNypBhCLsKNWueFgRyjEoXG3GvnXeZoY8ryJaWXrzsRZPxQ==";

        static readonly DocumentClient documentClient = new DocumentClient(new Uri(CosmosUrl), CosmosDbKey);

        public async Task<List<ProcessedImage>> GetAllProcessedImagesAsync()
        {
            var items = new List<ProcessedImage>();

            var query = documentClient.CreateDocumentQuery<ProcessedImage>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId)).AsDocumentQuery();
            while (query.HasMoreResults)
            {
                items.AddRange(await query.ExecuteNextAsync<ProcessedImage>());
            }

            return items;
        }

        public async Task<ProcessedImage> GetProcessedImage(string fileName)
        {
            var query = documentClient.CreateDocumentQuery<ProcessedImage>(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, fileName)).AsDocumentQuery();
            var feedResponse = await query.ExecuteNextAsync<ProcessedImage>();
            //feedResponse.
            return null;
        }
    }
}