using System;
using System.IO;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages
{
    public class ContentModModel : PageModel
    {
        private IAmazonRekognition rekognition;

        public ContentModModel(IAmazonRekognition rekognition)
        {
            this.rekognition = rekognition;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public DetectModerationLabelsResponse Result { get; set; }

        public async Task OnPostAsync()
        {
            using (var fileStream = Upload.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);

                    Result = await rekognition.DetectModerationLabelsAsync(new DetectModerationLabelsRequest
                    {
                        Image = new Image
                        {
                            Bytes = ms,
                        }
                    });
                }
            }
        }
    }
}
