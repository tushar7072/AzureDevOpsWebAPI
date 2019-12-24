using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;

namespace CreateWorkItems.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureWorkItemsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        //public ActionResult<IEnumerable<WorkItem>> Get()
        public async Task<string> Get()
        {
            await Security.GetSecretKeys();
            VssConnection connection = null;
            connection = new VssConnection(new Uri(Security.DevOpsAccount), new VssBasicCredential(string.Empty, Security.PAT));

            // Get a GitHttpClient to talk to the Git endpoints
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            //witClient.GetWorkItemsAsync()
            var wiqlQuery = new Wiql() { Query = "Select * from WorkItems" };
            
            var workItemQueryResultForWiqlBasedQuery = witClient.QueryByWiqlAsync(wiqlQuery).Result;

            var workItemsForQueryResultForWiqlBasedQuery = witClient
                .GetWorkItemsAsync(
                    workItemQueryResultForWiqlBasedQuery.WorkItems.Select(workItemReference => workItemReference.Id),
                    expand: WorkItemExpand.All).Result;

            JsonConvert.SerializeObject(workItemsForQueryResultForWiqlBasedQuery, Formatting.Indented);

            return JsonConvert.SerializeObject(workItemsForQueryResultForWiqlBasedQuery, Formatting.Indented); ;

        }

        [Route("api/[controller]/TestAPIFunction")]
        [HttpGet("TestAPIFunction")]
        public ActionResult<string> TestAPIFunction(string message)
        {
            return message;
        }

       

        [Route("api/[controller]/InsertWorkItem")]
        [HttpGet("InsertWorkItem")]
        public async Task<string> InsertWorkItem(string ProjectName, string Title, string AssignedTo, string Description)
        {
            //string account = "https://dev.azure.com/tusharpawar";

            await Security.GetSecretKeys();
            VssConnection connection = null;
            connection = new VssConnection(new Uri(Security.DevOpsAccount), new VssBasicCredential(string.Empty, Security.PAT));

            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var document = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();
            
            document.Add(
            new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
            {
                Path = @"/fields/System.Title",            
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                Value = Title
            });

            document.Add(
            new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
            {
                Path = @"/fields/System.AssignedTo",
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                Value = AssignedTo
            });

            document.Add(
            new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
            {
                Path = @"/fields/System.Description",
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                Value = Description
            });

            //var worktime = witClient.CreateWorkItemAsync(document, "ETR - Enterprise Tech Refresh", "Task").Result;
            var worktime = witClient.CreateWorkItemAsync(document, ProjectName, "Task").Result;

            return "Work Item Created!";
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
