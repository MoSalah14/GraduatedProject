using Microsoft.AspNetCore.Http;
using OutbornE_commerce.BAL.Dto;

namespace OutbornE_commerce.FilesManager
{
    public interface IFilesManager
    {
        Task<FileModel?> UploadFile(IFormFile? file, string tagName, string? oldFileUrl = null);

        Task<List<FileModel>> UploadMultipleFile(List<IFormFile> lstFiles, string tagName);

        bool DeleteFile(string fileUrl);

        bool DeleteMultipleFiles(List<string> filesUrl);
    }
}