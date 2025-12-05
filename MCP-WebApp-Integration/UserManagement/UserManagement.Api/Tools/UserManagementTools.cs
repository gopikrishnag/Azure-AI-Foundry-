using System.ComponentModel;
using ModelContextProtocol.Server;
using UserManagement.Model.Entities;

namespace UserManagement.Api.Tools
{
    public class UserManagementTools(ILogger<UserManagementTools> logger, UserManagementServices.UserService userService)
    {
        [McpServerTool, Description("Create a new user account")]
        public async Task<string> CreateNewUserAccount(
            [Description("Full name for the new account")] string fullName,
            [Description("Email address for the new account")] string emailAddress,
            [Description("Phone number for the new account")] string phoneNumber,
            [Description("Password for the new account")] string password)
        {
            try
            {
                var userId = await userService.CreateNewUserAccount(fullName, emailAddress, phoneNumber, password);
                return userId;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating new user account");
                throw;
            }

        }

        [McpServerTool, Description("Gets a list of all users")]
        public async Task<List<UserAccountEntity>> GetAllUsersAsync()
        {
            try
            {
                var userId = await userService.GetAllUserAccountsAsync();
                return userId;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in getting users list");
                throw;
            }

        }

        [McpServerTool, Description("Gets a single  user or perform user sign-in")]
        public async Task<UserAccountEntity> GetUserAsync(
            [Description("Email address for the new account")] string emailAddress,
            [Description("Password for the new account")] string password)
        {
            try
            {
                var user = await userService.GetUserAsync(emailAddress, password);
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in getting user");
                throw;
            }

        }
    }
}
