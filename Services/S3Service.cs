using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace backendcafe.Services
{
    public class AwsConfig
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }
    }

    public class S3Service : IS3Service
    {
        private readonly AmazonS3Client _s3Client;
        private readonly AwsConfig _awsConfig;

        public S3Service(IOptions<AwsConfig> awsConfig)
        {
            _awsConfig = awsConfig.Value;
            
            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_awsConfig.Region)
            };

            _s3Client = new AmazonS3Client(_awsConfig.AccessKey, _awsConfig.SecretKey, config);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            try
            {
                using var stream = file.OpenReadStream();
                
                var request = new PutObjectRequest
                {
                    BucketName = _awsConfig.BucketName,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };

                var response = await _s3Client.PutObjectAsync(request);
                
                // Return the public URL
                return $"https://{_awsConfig.BucketName}.s3.{_awsConfig.Region}.amazonaws.com/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to S3: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                // Extract key from URL
                var uri = new Uri(fileUrl);
                var key = uri.AbsolutePath.TrimStart('/');

                var request = new DeleteObjectRequest
                {
                    BucketName = _awsConfig.BucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking the main operation
                Console.WriteLine($"Error deleting file from S3: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _s3Client?.Dispose();
        }
    }
}