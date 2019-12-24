using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CreateAzureWorkItems.Models;
using System.Net.Http;

namespace CreateAzureWorkItems.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> GetAzureWorkItems()
        {
            string ReturnedJSON = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1455/api/");
                //HTTP GET
                var responseTask = client.GetAsync("AzureWorkItems");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    ReturnedJSON = await result.Content.ReadAsStringAsync();


                    //var alldata = readTask.Result;
                }

            }
            return ReturnedJSON;
        }

        public string TestMessage()
        {
            string message = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1455/api/azureworkitems/");
                //HTTP GET
                var responseTask = client.GetAsync(string.Format("TestAPIFunction?message={0}",
                                    "ETR - Enterprise Tech Refresh"));
                responseTask.Wait();

                var result = responseTask.Result;
            }

            return message;
        }

        public string AddAWorkItem()
        {
            string ReturnedJSON = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1455/api/azureworkitems/");
                //HTTP GET
                var responseTask = client.GetAsync(string.Format("InsertWorkItem?ProjectName={0}&Title={1}&AssignedTo={2}&Description={3}",
                                    "ETR - Enterprise Tech Refresh", "Remedy Issue 1", "tushar.pawar@gmail.com", "This is web api testing"));
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    ReturnedJSON = "Work Item Created Sucessfully";

                    //var alldata = readTask.Result;
                }

            }
            return ReturnedJSON;
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
