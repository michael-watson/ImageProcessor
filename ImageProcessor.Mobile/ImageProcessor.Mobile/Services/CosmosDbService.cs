using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;

namespace ImageProcessor.Mobile
{
	public class CosmosDbService
	{
		static readonly string DatabaseId = "metadata";
		static readonly string CollectionId = "analyzed-images";
		static readonly string CosmosUrl = "https://image-processing-miwats.documents.azure.com:443/";
		static readonly string CosmosDbKey = "anlvPs5x0UuzHBptPgANpN6yKOnsLwcD3IwTcyvgNypBhCLsKNWueFgRyjEoXG3GvnXeZoY8ryJaWXrzsRZPxQ==";

		static readonly DocumentClient documentClient = new DocumentClient(new Uri(CosmosUrl), CosmosDbKey);

		public static async Task<List<ProcessedImage>> GetAllProcessedImagesAsync()
		{
			var items = new List<ProcessedImage>();

			var query = documentClient.CreateDocumentQuery<ProcessedImage>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId)).AsDocumentQuery();
			while (query.HasMoreResults)
			{
				items.AddRange(await query.ExecuteNextAsync<ProcessedImage>());
			}

			return items;
		}
	}
}