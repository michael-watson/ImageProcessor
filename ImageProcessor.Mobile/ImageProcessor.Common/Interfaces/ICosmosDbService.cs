using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageProcessor.Common.Models;

namespace ImageProcessor.Common.Interfaces
{
    public interface ICosmosDbService
    {
        Task<List<ProcessedImage>> GetAllProcessedImagesAsync();
        Task<ProcessedImage> GetProcessedImage(string fileName);
    }
}