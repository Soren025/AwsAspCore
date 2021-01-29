using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages
{
    public class GameModel : PageModel
    {
        public string Color { get; set; }

        public IActionResult OnGet(string color)
        {
            if (color == "red" && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Cognito", new { message = "You need to be logged in to play the red game" });
            }

            Color = color ?? "none";

            return Page();
        }
    }
}
