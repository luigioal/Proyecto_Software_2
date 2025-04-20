using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Microsoft.Extensions.Configuration;
using Azure.Core;
using DTO.SeguridadDTO;

namespace AppLogic.ConnectorsAdmin
{
    public class AwsConnector
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        public AwsConnector(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _bucketName = config["AWS:BucketName"];
        }
        public PresignedUrlResponse GeneratePresignedUrl(string fileName)
        {
            var objectKey = $"uploads/{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            var presignedUrl = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(30),
            });

            var publicUrl = $"https://{_bucketName}.s3.amazonaws.com/{objectKey}";

            return new PresignedUrlResponse() { presignedUrl = presignedUrl, publicUrl =  publicUrl};
        }
    }
}
