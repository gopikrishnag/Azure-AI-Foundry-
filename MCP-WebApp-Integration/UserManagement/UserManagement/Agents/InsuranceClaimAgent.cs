using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Agents.Persistent;
using Microsoft.Extensions.Configuration;
using UserManagement.Model.Entities;

namespace UserManagementService.Agents
{
    public class InsuranceClaimAgent
    {

        private readonly PersistentAgentsClient persistentAgentsClient;
        private readonly string agentId = string.Empty;
        public InsuranceClaimAgent(PersistentAgentsClient persistentAgentsClient, string agentId)
        {
            this.persistentAgentsClient = persistentAgentsClient;
            this.agentId = agentId;  
        }

        public async Task<string> CreateInsurancePolicyAsync(InsuranceClaimModel claimDetails)
        {
            string prompt = $"""
Create a new insurance claim with  the following details:
content: {claimDetails.Content}
insurer: {claimDetails.Insurer}
title: {claimDetails.Title}
tags: {claimDetails.Tags.FirstOrDefault()}
premiumAmount: {claimDetails.PremiumAmount}
isActive: {claimDetails.IsActive}
""";
            string apiResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgentAsync(prompt, agentId);
            return apiResponse;

        }
    }
}
