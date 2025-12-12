using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using StorageAccountTableService;
using UserManagement.Model.Entities;
using UserManagementService;


namespace UserManagement.Api.Tools
{
    public class InsurancePolicyTools(ILogger<InsurancePolicyTools> logger, StorageAccountBlobService storageAccountBlobService )
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

              await storageAccountBlobService.UploadDocuments(new List<AzureSearchDocument>()
                {
                    new()
                    {
                        Id=Guid.NewGuid().ToString(),
                        Content=content,
                        Insurer=insurer,
                        Title=title,
                        Tags=tags.Split(',').Select(t=>t.Trim()).ToArray(),
                        PremiumAmount=premiumAmount,
                        IsActive=isActive
                    }
                });
                return   $"New insurance policy created for  {title} by {insurer}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating new Insurance record");
                throw;
            }

        }
    }
}
