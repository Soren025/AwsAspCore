using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages
{
    public class GameModel : PageModel
    {
        public string Color { get; set; }

        public void OnGet(string color)
        {
            Color = color ?? "none";
        }
    }
}
