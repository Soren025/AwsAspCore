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

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IActionResult OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Cognito", new { message = "You need to be logged in to upload images" });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Cognito", new { message = "You need to be logged in to upload images" });
            }

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
