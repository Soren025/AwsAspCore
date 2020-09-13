using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AwsAspCore.Models;
using Amazon.DynamoDBv2;
using AwsAspCore.DDB;

namespace AwsAspCore.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private IAmazonDynamoDB ddb;

        public CreateModel(IAmazonDynamoDB ddb)
        {
            this.ddb = ddb;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Movie Movie { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Movie.ID = Guid.NewGuid();
            await ddb.Movies_Update(Movie);

            return RedirectToPage("./Index");
        }
    }
}
