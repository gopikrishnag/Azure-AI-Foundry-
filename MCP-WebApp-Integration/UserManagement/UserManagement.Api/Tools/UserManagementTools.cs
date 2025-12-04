using System.ComponentModel;
using ModelContextProtocol.Server;

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
    }
}
