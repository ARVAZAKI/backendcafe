    public interface IS3Service
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
        Task DeleteFileAsync(string fileUrl);
    }