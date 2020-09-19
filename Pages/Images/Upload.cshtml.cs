using System;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages.Images
{
    public class UploadModel : PageModel
    {
        private IAmazonS3 s3;
        private string bucketName;

        public UploadModel(IAmazonS3 s3)
        {
            this.s3 = s3;
            bucketName = Environment.GetEnvironmentVariable("IMAGE_STORAGE_BUCKET_NAME");
        }

        // IMPORTANT
        // Out of the box this won't work
        // Api Gateway out of the box malforms the file binary corrupting it
        // Follow the steps here: https://stackoverflow.com/a/61752279/3159342 to make it work
        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            using (var fileStream = Upload.OpenReadStream())
            {
                await s3.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = Upload.FileName,
                    InputStream = fileStream,
                    ContentType = Upload.ContentType,
                });
            }

            return RedirectToPage("./Index");
        }
    }
}
