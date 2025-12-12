using Azure.AI.Agents.Persistent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserManagement.Model.Entities;

namespace UserManagementService.Agents
{
    public class UserManagementAgent
    {
        private readonly PersistentAgentsClient persistentAgentsClient;
        private readonly string agentId = string.Empty;
        public UserManagementAgent(PersistentAgentsClient persistentAgentsClient, string agentId)
        {
            this.persistentAgentsClient = persistentAgentsClient;
            this.agentId = agentId;

        }

        public async Task<string> CreateUserAccountAsync(UserAccountModel createNewUserAccountModel)
        {
            string prompt = $"""
Create a new user account with the following details:
Full Name: {createNewUserAccountModel.FullName}
Email Address: {createNewUserAccountModel.EmailAddress}
Phone Number: {createNewUserAccountModel.PhoneNumber}
Password: {createNewUserAccountModel.Password}
""";
            string apiResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgentAsync(prompt, agentId);
            return apiResponse;

        }



        public async Task<string> GetAllUsersAsync()
        {
            string prompt = "Get a list of all users and make sure you return as raw json and don't add any additional or extra text in the returned text. I have to parse the returned json";
            string apiResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgentAsync(prompt, agentId);
            return apiResponse;
        }

        public async Task<string> GetUserAsync(string email, string password)
        {
            string prompt = $"Get a single user based on provide information email address - {email} and password - {password} and make sure you return as raw json and don't add any additional or extra text in the returned text. I have to parse the returned json";
            string apiResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgentAsync(prompt, agentId);
            return apiResponse;
        }



    }
}
