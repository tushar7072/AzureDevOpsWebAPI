using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        [ResponseCache(Duration = 60)]
        //public ActionResult<IEnumerable<WorkItem>> Get()
        public async Task<IActionResult> Get()
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

            //JsonConvert.SerializeObject(workItemsForQueryResultForWiqlBasedQuery, Formatting.Indented);

            return StatusCode(StatusCodes.Status200OK, workItemsForQueryResultForWiqlBasedQuery);

        }

        [Route("api/[controller]/TestAPIFunction")]
        [HttpGet("TestAPIFunction")]
        public ActionResult<string> TestAPIFunction(string message)
        {
            return message;
        }



        [Route("api/[controller]/InsertWorkItem")]
        [HttpPost("InsertWorkItem")]
        //public async Task<string> InsertWorkItem(string ProjectName, string Title, string AssignedTo, string Description)
        public async Task<IActionResult> InsertWorkItem([FromBody] Models.WorkItem WI)
        {
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
                Value = WI.Title
            });

            document.Add(
            new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
            {
                Path = @"/fields/System.AssignedTo",
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                Value = WI.AssignedTo
            });

            document.Add(
            new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
            {
                Path = @"/fields/System.Description",
                Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                Value = WI.Description
            });

            var worktime = witClient.CreateWorkItemAsync(document, WI.ProjectName, "Task").Result;

            return StatusCode(StatusCodes.Status201Created, "Work Item with Id " + worktime.Id + " created sucessfully!");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

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
