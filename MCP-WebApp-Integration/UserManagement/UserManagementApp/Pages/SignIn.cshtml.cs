using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Model.Entities;
using UserManagementService.Agents;

namespace UserManagementApp.Pages
{
    public class SignInModel : PageModel
    {
        [BindProperty]
        public UserSignInModel UserSignIn { get; set; } = new();
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        private readonly UserManagementAgent userManagementAgent;

        public SignInModel(UserManagementAgent userManagementAgent)
        {
            this.userManagementAgent = userManagementAgent;
        }
        public void OnGet()
        {
        }
        public async Task  OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
               // return Page();
            }
            try
            {
                SuccessMessage = string.Empty;
                ErrorMessage = string.Empty;
                var usersResponse = await userManagementAgent.GetUserAsync(UserSignIn.EmailAddress, UserSignIn.Password);
                var User = System.Text.Json.JsonSerializer.Deserialize<UserAccountModel>(usersResponse, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
                if (User?.EmailAddress != null)
                {
                    SuccessMessage = "Sign In successful!";
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    // ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while creating the account: {ex.Message}");
              //  return Page();
            }
        }
    }
}
