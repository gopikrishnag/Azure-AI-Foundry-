using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Agents.Persistent;
using UserManagement.Model.Entities;
using Microsoft.Extensions.Configuration;

namespace UserManagementService.Agents
{
    public class UserManagementAgent
    {
        private readonly PersistentAgentsClient persistentAgentsClient;
        private readonly IConfiguration configuration;
        public UserManagementAgent(IConfiguration configuration)
        {
            this.configuration = configuration;
            string foundryEndpoint = configuration["AZURE_FOUNDRY_ENDPOINT"];
            persistentAgentsClient = new PersistentAgentsClient(foundryEndpoint, new Azure.Identity.DefaultAzureCredential());
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
            string apiResponse = await CallUserManagementApiAsync(prompt);
            return apiResponse;


        }



        public async Task<string> GetAllUsersAsync()
        {
            string prompt = "Get a list of all users and make sure you return as raw json and don't add any additional or extra text in the returned text. I have to parse the returned json";
            string apiResponse = await CallUserManagementApiAsync(prompt);
            return apiResponse;
        }


        //TODO: convert into Async
        private async Task<string> CallUserManagementApiAsync(string prompt)
        {
            string agentId = configuration["FOUNDRY_AGENT_ID"];
            PersistentAgentThread agentThread = persistentAgentsClient.Threads.CreateThread();
            var agent = persistentAgentsClient.Administration.GetAgent(agentId);
            var message = persistentAgentsClient.Messages.CreateMessage(agentThread.Id, MessageRole.User, prompt);
            var threadRun = persistentAgentsClient.Runs.CreateRun(agentThread.Id, agent.Value.Id);

            do
            {

                await Task.Delay(2000);
                threadRun = persistentAgentsClient.Runs.GetRun(agentThread.Id, threadRun.Value.Id);

                if (threadRun.Value.Status == RunStatus.RequiresAction)
                {
                    if (threadRun.Value.RequiredAction is SubmitToolApprovalAction toolApprovalAction)
                    {
                        var approvedToolCalls = new List<ToolApproval>();
                        foreach (var toolCall in toolApprovalAction.SubmitToolApproval.ToolCalls)
                        {
                            if (toolCall is RequiredMcpToolCall mcpToolCall)
                            {
                                approvedToolCalls.Add(new ToolApproval(toolCall.Id, true));
                            }
                        }
                        await persistentAgentsClient.Runs.SubmitToolOutputsToRunAsync(agentThread.Id, threadRun.Value.Id, null, approvedToolCalls);

                    }
                }

                if (threadRun.Value.Status == RunStatus.Completed)
                {
                    break;
                }

            }
            while (threadRun.Value.Status != RunStatus.Queued || threadRun.Value.Status != RunStatus.InProgress || threadRun.Value.Status == RunStatus.RequiresAction);

            var agentMessages = persistentAgentsClient.Messages.GetMessagesAsync(agentThread.Id);
            await foreach (var agentMessage in agentMessages)
            {
                foreach (var msgContent in agentMessage.ContentItems)
                {
                    if (msgContent is MessageTextContent textContent)
                    {
                        return textContent.Text;
                    }
                }
            }

            return "";
        }
    }
}
