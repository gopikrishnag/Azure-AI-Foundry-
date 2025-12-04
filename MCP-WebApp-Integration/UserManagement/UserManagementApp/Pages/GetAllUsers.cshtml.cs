using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Model.Entities;
using UserManagementService.Agents;

namespace UserManagementApp.Pages
{
    public class GetAllUsersModel : PageModel
    {
        //[BindProperty]
        public List<UserAccountModel> Users { get; set; } = [];

        private readonly UserManagementAgent userManagementAgent;

        public GetAllUsersModel(UserManagementAgent userManagementAgent)
        {
            this.userManagementAgent = userManagementAgent;
        }
        public async Task OnGet()
        {
            try
            {
                var usersResponse = await userManagementAgent.GetAllUsersAsync();
                Users = System.Text.Json.JsonSerializer.Deserialize<List<UserAccountModel>>(usersResponse, new System.Text.Json.JsonSerializerOptions() { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase }) ?? [];

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while get the list of users: {ex.Message}");
            }
        }
    }
}
