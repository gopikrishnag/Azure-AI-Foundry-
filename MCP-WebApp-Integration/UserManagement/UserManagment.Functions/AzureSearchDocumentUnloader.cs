using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UserManagementService;

namespace UserManagment.Functions
{
    public class AzureSearchDocumentUnloader
    {
        private readonly ILogger<AzureSearchDocumentUnloader> _logger;
        private readonly DocumentService _documentService;

        public AzureSearchDocumentUnloader(ILogger<AzureSearchDocumentUnloader> logger, DocumentService documentService)
        {
            _logger = logger;
            _documentService = documentService;
        }

        [Function("AzureSearchDocumentUnloader")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            //var docs = _documentService.GenerateHealthInsuranceDocuments(10);
            // await _documentService.UploadDocuments(docs);

           // await _documentService.CreateOrUpdateIndex();
            await _documentService.CreateOrUpdateIndexer();

            return req.CreateResponse(System.Net.HttpStatusCode.OK);

        }
    }
}
