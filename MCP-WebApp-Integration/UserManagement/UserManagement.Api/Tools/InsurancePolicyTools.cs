using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using StorageAccountTableService;


namespace UserManagement.Api.Tools
{
    public class InsurancePolicyTools
    {
        [McpServerTool, Description("Create a new insurance policy")]
        public async Task<string> CreateNewInsurancePolicy(
           [Description("Content of the policy")] string content,
           [Description("Insurer of the policy")] string insurer,
           [Description("Title of the policy")] string title,
           [Description("Tags for the policy, comma separated string")] string tags,
           [Description("Premium Amount of the policy")] double premiumAmount,
           [Description("IsActive, described if policy is active or not")] bool isActive)
        {
            try
            {
               // var userId = await userService.CreateNewUserAccount(fullName, emailAddress, phoneNumber, password);
                return "userId";
            }
            catch (Exception ex)
            {
               // logger.LogError(ex, "Error creating new user account");
                throw;
            }

        }
    }
}
