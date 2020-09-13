using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AwsAspCore.Models;
using Amazon.DynamoDBv2;
using AwsAspCore.DDB;

namespace AwsAspCore.Pages.Movies
{
    public class IndexModel : PageModel
    {
        IAmazonDynamoDB ddb;

        public IndexModel(IAmazonDynamoDB ddb)
        {
            this.ddb = ddb;
        }

        public IList<Movie> Movie { get;set; }

        public async Task OnGetAsync(string sortBy)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "title";
            }

            switch (sortBy)
            {
                case "title":
                default:
                    Movie = await ddb.Movies_GetAll(movie => movie.Title, StringComparer.OrdinalIgnoreCase);
                    break;
                case "releaseDate":
                    Movie = await ddb.Movies_GetAll(movie => movie.ReleaseDate);
                    break;
                case "genre":
                    Movie = await ddb.Movies_GetAll(movie => movie.Genre, StringComparer.OrdinalIgnoreCase);
                    break;
                case "price":
                    Movie = await ddb.Movies_GetAll(movie => movie.Price);
                    break;
            }

        }
    }
}
