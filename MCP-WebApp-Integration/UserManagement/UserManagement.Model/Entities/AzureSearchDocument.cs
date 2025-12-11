

using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace UserManagement.Model.Entities
{
    public class AzureSearchDocument
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; }
        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Content { get; set; }
        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Insurer { get; set; }
        [SimpleField]
        public string Title { get; set; }
        [SimpleField(IsFacetable =true, IsFilterable = true)]
        public string[] Tags { get; set; }
        public double PremiumAmount { get; set; }
        public bool IsActive { get; set; }
        [SimpleField(IsFilterable = false, IsSortable = false, IsFacetable =false)]
        [VectorSearchField(VectorSearchDimensions =1536, VectorSearchProfileName ="vector-profile")]
        public float[]   ContentVector { get; set; }

    }
}
