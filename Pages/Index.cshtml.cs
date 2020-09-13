using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;

using AwsAspCore.DDB;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AwsAspCore.Pages
{
    public class IndexModel : PageModel
    {
        IAmazonDynamoDB ddb;

        public IndexModel(IAmazonDynamoDB ddb)
        {
            this.ddb = ddb;
        }

        public int MovieCount { get; set; }

        public string Message { get; set; }

        public async Task OnGetAsync(string message)
        {
            Message = message ?? "";
            MovieCount = await ddb.Movies_Count();
        }

        //public async Task OnGetHelloWorldAsync()
        //{
        //    Message = "Hello World";
        //    MovieCount = await ddb.Movies_Count();
        //}
    }
}