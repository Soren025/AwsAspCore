using System;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;

using AwsAspCore.DDB;
using AwsAspCore.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages.Movies
{
    public class DeleteModel : PageModel
    {
        private IAmazonDynamoDB ddb;

        public DeleteModel(IAmazonDynamoDB ddb)
        {
            this.ddb = ddb;
        }

        [BindProperty]
        public Movie Movie { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie = await ddb.Movies_Get(id.Value);

            if (Movie == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await ddb.Movies_Remove(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
