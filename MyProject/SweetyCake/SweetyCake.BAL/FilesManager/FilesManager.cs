using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OutbornE_commerce.BAL.Dto;
using Serilog;
using System.Linq;
using System.Net;

namespace OutbornE_commerce.FilesManager
{
    public class FilesManager : IFilesManager
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilesManager(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<FileModel?> UploadFile(IFormFile? file, string tagName, string? oldFileUrl = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Exception("No file was provided.");
                }

                var fileExtension = Path.GetExtension(file.FileName)?.ToLower();

                var allowedExtensions = new[] { ".jpg",".png","jpeg" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new Exception("Invalid file type. Only .jpg");
                }
                if (!string.IsNullOrEmpty(oldFileUrl))
                {
                    var BaseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/";
                    if (oldFileUrl.StartsWith(BaseUrl))
                    {
                        oldFileUrl = oldFileUrl.Replace(BaseUrl, string.Empty);
                    }

                    string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, oldFileUrl.Replace("/", "\\"));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                string schema = "https";
                var request = _httpContextAccessor.HttpContext!.Request;
                //if (!request.IsHttps)
                //{
                //    schema = "http";
                //}

                var defaultBaseUrl = $"{schema}://{request.Host}";
                string path = "Uploads/" + tagName + "/";
                string physicalFilePath = Path.Combine(_hostingEnvironment.WebRootPath, path);
                if (!Directory.Exists(physicalFilePath))
                {
                    Directory.CreateDirectory(physicalFilePath);
                }

                string fileName = Guid.NewGuid() + fileExtension;
                using (var stream = new FileStream(Path.Combine(physicalFilePath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                FileModel fileModel = new FileModel
                {
                    Url = defaultBaseUrl + "/" + path + fileName,
                    FileName = file.FileName
                };
                return fileModel;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<FileModel>> UploadMultipleFile(List<IFormFile> lstFiles, string tagName)
        {
            try
            {
                var allowedExtensions = ".jpg";
                foreach (var FileNamePath in lstFiles)
                {
                    if (!FileNamePath.FileName.EndsWith(allowedExtensions))
                        return null;
                }

                List<FileModel> fileModelList = new List<FileModel>();

                string schema = "https";
                var request = _httpContextAccessor.HttpContext!.Request;

                if (!request.IsHttps)
                    schema = "http";

                var baseUrl = $"{schema}://{request.Host}";

                string path = "Uploads/" + tagName + "/";
                string PhysicalfilePath = Path.Combine(_hostingEnvironment.WebRootPath, path);

                if (!Directory.Exists(PhysicalfilePath))
                    Directory.CreateDirectory(PhysicalfilePath);

                foreach (IFormFile currentFile in lstFiles)
                {
                    var fileExtension = Path.GetExtension(currentFile.FileName)?.ToLower();
                    FileModel fileModel = new FileModel();
                    string fileName = Guid.NewGuid() + fileExtension;
                    using (var stream = new FileStream(PhysicalfilePath + fileName, FileMode.Create))
                    {
                        await currentFile.CopyToAsync(stream);
                    }

                    fileModel.Url = baseUrl + "/" + path + fileName;
                    fileModel.FileName = currentFile.FileName;
                    fileModelList.Add(fileModel);
                }
                return fileModelList;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool DeleteFile(string fileUrl)
        {
            try
            {
                var baseUrl = $"https://{_httpContextAccessor.HttpContext.Request.Host}/";

                Log.Information("Base URL: {BaseUrl}", baseUrl);
                Log.Information("Original File URL: {FileUrl}", fileUrl);

                // Remove the base URL part from the file URL to get the relative path
                if (fileUrl.StartsWith(baseUrl))
                {
                    fileUrl = fileUrl.Replace(baseUrl, string.Empty);
                    Log.Information("File URL after base URL removal: {FileUrl}", fileUrl);
                }

                // Ensure the fileUrl starts from a relative path (remove leading '/')
                string relativePath = fileUrl.TrimStart('/');
                string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                Log.Information("Computed File Path: {OldFilePath}", oldFilePath);

                // Check if the file exists at the computed path
                if (File.Exists(oldFilePath))
                {
                    Log.Information("File found, attempting to delete: {OldFilePath}", oldFilePath);
                    File.Delete(oldFilePath);
                    Log.Information("File deleted successfully: {OldFilePath}", oldFilePath);
                    return true;
                }
                else
                {
                    Log.Warning("File not found: {OldFilePath}", oldFilePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during file deletion for URL: {FileUrl}", fileUrl);
                return false;
            }
        }

        public bool DeleteMultipleFiles(List<string> fileUrls)
        {
            var BaseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/";
            var filePaths = new List<string>();

            // Check if all files exist and collect their paths
            foreach (var fileUrl in fileUrls)
            {
                string tempFileUrl = fileUrl;

                // Remove base URL
                if (tempFileUrl.StartsWith(BaseUrl))
                    tempFileUrl = tempFileUrl.Replace(BaseUrl, string.Empty);

                string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, tempFileUrl.Replace("/", "\\"));

                if (!File.Exists(oldFilePath))
                    return false;

                filePaths.Add(oldFilePath);
            }
            try
            {
                foreach (var filePath in filePaths)
                    File.Delete(filePath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] ExportSalesReportToExcelAsync(List<SalesReportDto> reportData)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Add New Sheet
                var worksheet = package.Workbook.Worksheets.Add("Sales Report");

                // Set header row
                string[] headers = new[] {
                    "Start Date", "End Date", "Period", "Brand Number", "Brand Name",
                    "Product SKU", "Product Name", "Status", "Date Added", "Retail Price",
                    "Period Start Inventory", "Period End Inventory", "Period Sales Count",
                    "Period Sales Amount", "Total Sales Count", "Total Sales Amount",
                    "Period Returns Count", "Period Returns Amount"
                    };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Populate data rows
                for (int row = 0; row < reportData.Count; row++)
                {
                    var data = reportData[row];
                    int excelRow = row + 2; // Data starts from row 2

                    worksheet.Cells[excelRow, 1].Value = data.StartDate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[excelRow, 2].Value = data.EndDate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[excelRow, 3].Value = data.Period;
                    worksheet.Cells[excelRow, 4].Value = data.BrandNumber;
                    worksheet.Cells[excelRow, 5].Value = data.BrandName;
                    worksheet.Cells[excelRow, 6].Value = data.ProductSKU;
                    worksheet.Cells[excelRow, 7].Value = data.ProductName;
                    worksheet.Cells[excelRow, 8].Value = data.Status;
                    worksheet.Cells[excelRow, 9].Value = data.DateAdded?.ToString("yyyy-MM-dd");
                    worksheet.Cells[excelRow, 10].Value = data.RetailPrice;
                    worksheet.Cells[excelRow, 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[excelRow, 11].Value = data.PeriodStartInventory;
                    worksheet.Cells[excelRow, 12].Value = data.PeriodEndInventory;
                    worksheet.Cells[excelRow, 13].Value = data.PeriodSalesCount;
                    worksheet.Cells[excelRow, 14].Value = data.PeriodSalesAmount;
                    worksheet.Cells[excelRow, 14].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[excelRow, 15].Value = data.TotalSalesCount;
                    worksheet.Cells[excelRow, 16].Value = data.TotalSalesAmount;
                    worksheet.Cells[excelRow, 16].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[excelRow, 17].Value = data.PeriodReturnsCount;
                    worksheet.Cells[excelRow, 18].Value = data.PeriodReturnsAmount;
                    worksheet.Cells[excelRow, 18].Style.Numberformat.Format = "#,##0.00";

                    // Apply border to each cell in the row
                    for (int col = 1; col <= headers.Length; col++)
                    {
                        worksheet.Cells[excelRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                }

                // Auto-fit columns for all data
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save to a byte array and return it
                return package.GetAsByteArray();
            }
        }
    }
}