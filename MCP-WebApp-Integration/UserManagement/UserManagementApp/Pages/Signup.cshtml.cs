using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Model.Entities;
using UserManagementService.Agents;

namespace UserManagementApp.Pages
{
    public class SignUpModel : PageModel
    {

        [BindProperty]
        public UserAccountModel UserInput { get; set; } = new();
        private readonly UserManagementAgent userManagementAgent;

        public SignUpModel(UserManagementAgent userManagementAgent)
        {
            this.userManagementAgent = userManagementAgent;
        }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                await userManagementAgent.CreateUserAccountAsync(UserInput);
                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while creating the account: {ex.Message}");
                return Page();
            }
        }
    }
}
