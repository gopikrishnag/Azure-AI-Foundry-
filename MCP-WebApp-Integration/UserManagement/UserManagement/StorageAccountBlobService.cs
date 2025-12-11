using System.Text.Json;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;
using UserManagement.Model.Entities;
namespace UserManagementService
{
    public class StorageAccountBlobService
    {
        private string storageAccountConnectionString;
        private string searchEndPoint;
        private string searchKey;
        private readonly EmbeddingClient embeddingClient;
        private readonly SearchClient searchClient;
        public StorageAccountBlobService(IConfiguration configuration)
        {
            storageAccountConnectionString = configuration["AzureWebBlobStorage"];
            var client = new OpenAIClient(new Azure.AzureKeyCredential(configuration["AZURE_OPEN_API_KEY"])
                , new OpenAIClientOptions() { Endpoint = new Uri($"{configuration["AZURE_OPEN_API_ENDPOINT"]}/models") });
            embeddingClient = client.GetEmbeddingClient("text-embedding-ada-002");
            searchEndPoint = configuration["AZURE_SEARCH_ENDPOINT"];
            searchKey = configuration["AZURE_SEARCH_KEY"];

            searchClient = new SearchClient(new Uri(searchEndPoint), "insurance-claim-index", new AzureKeyCredential(searchKey));
        }

        public List<AzureSearchDocument>  GenerateHealthInsuranceDocuments(int documentCount)
        {
            var coverageTypes = new List<string>
            {
                "hospitalization", "prescription drugs", "dental", "vision", "preventive care", "mental health", "maternity"
            };

            var exclusions = new List<string>
            {
                "pre-existing conditions", "experimental treatments", "cosmetic procedures", "alternative therapies"
            };
            var extras = new List<string>
            {
                "wellness programs", "telemedicine services", "fitness memberships", "nutritional counseling", "dental care"
            };
            var faker = new Bogus.Faker<AzureSearchDocument>()
                .RuleFor(d => d.Id, f => f.Random.Guid().ToString())
                .RuleFor(d => d.Title, f => f.Company.CatchPhrase())
                .RuleFor(d => d.Insurer, f => $"{f.Company.CompanyName()} insurance")
                .RuleFor(d => d.Tags, f => f.PickRandom(coverageTypes, f.Random.Int(2, 5)).ToArray())
                .RuleFor(d => d.PremiumAmount, f => (double)Math.Round(f.Finance.Amount(100, 1000), 2))
                .RuleFor(d => d.IsActive, f => f.Random.Bool(0.8f))
                 .RuleFor(f => f.Content, d =>
                 {
                     var sentences = new List<string>()
                    {
                        $"This policy covers {d.PickRandom(coverageTypes)} for eligible customers",
                        $"Optional extras include {d.PickRandom(extras)}",
                        $"Exclusions include {d.PickRandom(exclusions)} are not covered",
                        $"Policy is provided by {d.Company.CompanyName()} Insurance"
                    };
                     return string.Join(" ", sentences.OrderBy(x => d.Random.Int()));
                 });

                 var documents = faker.Generate(documentCount);
            return documents;
        }
        public async Task UploadDocuments(List<AzureSearchDocument> documents)
        {
            var containerClient = new BlobContainerClient(storageAccountConnectionString, "rag-documents");
            foreach (var doc in documents)
            {
                doc.ContentVector = await GetEmbeddings(doc.Content);
                var blobClient = containerClient.GetBlobClient($"{doc.Id}.json");
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(doc, new JsonSerializerOptions() { WriteIndented = true });
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
                await blobClient.UploadAsync(stream, overwrite: true);
            }
        }

        public async Task<float[]> GetEmbeddings(string text)
        {
            var response = await embeddingClient.GenerateEmbeddingAsync(text);
            return response.Value.ToFloats().ToArray();
        }

        public async Task CreateOrUpdateIndex()
        {
            var searchIndex = new SearchIndex("insurance-claim-index")
            {
                Fields = new FieldBuilder().Build(typeof(AzureSearchDocument)),
                VectorSearch = new VectorSearch
                {
                    Profiles = { new VectorSearchProfile("vector-profile", "vector-algorithm") },
                    Algorithms = { new HnswAlgorithmConfiguration("vector-algorithm") }
                }
            };

            AzureKeyCredential azureKeyCredential = new AzureKeyCredential(searchKey);
            SearchIndexClient searchIndexClient = new SearchIndexClient(new Uri(searchEndPoint), azureKeyCredential);
            await searchIndexClient.CreateOrUpdateIndexAsync(searchIndex);
        }

        public async Task CreateOrUpdateIndexer()
        {
            var indexerClient = new SearchIndexerClient(new Uri(searchEndPoint), new AzureKeyCredential(searchKey));
            var indexer = new SearchIndexer(name: "insurance-documents-indexer", "insurce-blob-storage", "insurance-claim-index")
            {
                Parameters = new IndexingParameters
                {
                    IndexingParametersConfiguration = new IndexingParametersConfiguration
                    {
                        ["parsingMode"] = "json"
                    }
                },
                FieldMappings =
                {
                    new FieldMapping("Id"){TargetFieldName="Id"},
                    new FieldMapping("Content"){TargetFieldName="Content"},
                    new FieldMapping("Insurer"){TargetFieldName="Insurer"},
                    new FieldMapping("Title"){TargetFieldName="Title"},
                    new FieldMapping("Tags"){TargetFieldName="Tags"},
                    new FieldMapping("PremiumAmount"){TargetFieldName="PremiumAmount"},
                    new FieldMapping("IsActive"){TargetFieldName="IsActive"},
                    new FieldMapping("ContentVector"){TargetFieldName="ContentVector"}
                }
            };
            await indexerClient.CreateOrUpdateIndexerAsync(indexer);
        }

        public async Task<Response<SearchResults<SearchDocument>>> SearchDocuments(string queryText, SearchOptions searchOptions)
        {
            var searchResult = await searchClient.SearchAsync<SearchDocument>(queryText, searchOptions);
            return searchResult;
        }

    }
}
