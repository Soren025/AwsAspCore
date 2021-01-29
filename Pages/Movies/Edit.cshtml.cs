using System;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;

using AwsAspCore.DDB;
using AwsAspCore.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages.Movies
{
    public class EditModel : PageModel
    {
        private IAmazonDynamoDB ddb;

        public EditModel(IAmazonDynamoDB ddb)
        {
            this.ddb = ddb;
        }

        [BindProperty]
        public Movie Movie { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Cognito", new { message = "You need to be logged in to edit a movie" });
            }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Cognito", new { message = "You need to be logged in to edit a movie" });
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await ddb.Movies_Update(Movie);

            return RedirectToPage("./Index");
        }
    }
}
