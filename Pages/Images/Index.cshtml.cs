using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages.Images
{
    public class IndexModel : PageModel
    {
        private IAmazonS3 s3;
        private string bucketName;

        public IndexModel(IAmazonS3 s3)
        {
            this.s3 = s3;
            bucketName = Environment.GetEnvironmentVariable("IMAGE_STORAGE_BUCKET_NAME");
        }

        public List<string> Images { get; set; }

        public async Task OnGetAsync()
        {
            ListObjectsV2Response response;
            List<S3Object> result = new List<S3Object>();

            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
            };

            do
            {
                response = await s3.ListObjectsV2Async(request);
                request.ContinuationToken = response.ContinuationToken;
                result.AddRange(response.S3Objects);
            }
            while (!string.IsNullOrEmpty(response.ContinuationToken));

            Images = result.Select(obj => s3.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = obj.Key,
                Expires = DateTime.UtcNow.AddMinutes(10),
            }))
            .ToList();
        }

        public async Task<IActionResult> OnPostDeleteAllAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Cognito", new { message = "You need to be logged in to delete all the images" });
            }

            ListObjectsV2Response response;
            List<S3Object> result = new List<S3Object>();

            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
            };

            do
            {
                response = await s3.ListObjectsV2Async(request);
                request.ContinuationToken = response.ContinuationToken;
                result.AddRange(response.S3Objects);
            }
            while (!string.IsNullOrEmpty(response.ContinuationToken));

            if (result.Count > 0)
            {
                await s3.DeleteObjectsAsync(new DeleteObjectsRequest
                {
                    BucketName = bucketName,
                    Quiet = true,
                    Objects = result.Select(obj => new KeyVersion
                    {
                        Key = obj.Key,
                    })
                    .ToList(),
                });
            }

            return RedirectToPage("./Index");
        }
    }
}
