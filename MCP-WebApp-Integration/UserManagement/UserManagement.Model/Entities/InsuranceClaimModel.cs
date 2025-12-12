

using System.ComponentModel.DataAnnotations;

namespace UserManagement.Model.Entities
{
    public class InsuranceClaimModel
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
        public string Insurer { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }
        public double PremiumAmount { get; set; }
        public bool IsActive { get; set; }

    }
}
