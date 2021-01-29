using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Amazon.AspNetCore.Identity.Cognito;
using Amazon.DynamoDBv2;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Lambda.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages
{
    public class CognitoModel : PageModel
    {
        private SignInManager<CognitoUser> signInManager;
        private CognitoUserManager<CognitoUser> userManager;
        private RoleManager<CognitoRole> roleManager;

        public CognitoModel(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, RoleManager<CognitoRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager as CognitoUserManager<CognitoUser>;
            this.roleManager = roleManager;
        }

        public string Message { get; set; }

        public bool SignedIn { get; set; }

        public CognitoUser CUser { get; set; }

        public async Task OnGetAsync(string message = null)
        {
            Message = HttpUtility.UrlDecode(message);
            SignedIn = signInManager.IsSignedIn(User);
            CUser = await userManager.GetUserAsync(User);
        }

        public async Task<IActionResult> OnPostLoginAsync(string username, string password, bool rememberMe = false)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, rememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToPage("/Cognito", new { message = "Login Success!" });
            }
            else
            {
                return RedirectToPage("/Cognito", new { message = "Login Failed!" });
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            return RedirectToPage("/Cognito", new { message = "Logged Out!" });
        }

        public async Task<IActionResult> OnPostToggleRedRoleAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (await userManager.IsInRoleAsync(user, "red"))
            {
                await userManager.RemoveFromRoleAsync(user, "red");
                return RedirectToPage("/Cognito", new { message = "No longer in the red role. You will no longer be able to play the red game when you next log in" });
            }
            else
            {
                await userManager.AddToRoleAsync(user, "red");
                return RedirectToPage("/Cognito", new { message = "Your now in the red role! Relog to play the red game." });
            }
        }
    }
}
