using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagement.Model.Entities;
using UserManagementService.Agents;

namespace UserManagementApp.Pages
{
    public class NewInsurancePolicyModel : PageModel
    {
        [BindProperty]
        public InsuranceClaimModel InsurancePolicyModel  { get; set; } = new();
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public InsuranceClaimAgent insuranceClaimAgent;

        public NewInsurancePolicyModel(InsuranceClaimAgent insuranceClaimAgent)
        {
            this.insuranceClaimAgent = insuranceClaimAgent;

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
              var response =   await insuranceClaimAgent.CreateInsurancePolicyAsync(InsurancePolicyModel);
                TempData["SuccessMessage"] = $"Account created successfully! {response}";
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
