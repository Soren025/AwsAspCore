using System;

using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Rekognition;
using Amazon.S3;

using AwsAspCore.DDB.Caching;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AwsAspCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Domain = "awsaspcore.codari.co";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // Testing out using SSM as data protection for session keys
            // https://aws.amazon.com/blogs/developer/aws-ssm-asp-net-core-data-protection-provider/
            services.AddDataProtection()
                .PersistKeysToAWSSystemsManager("/Codari/AwsAspCore/DataProtection");

            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonRekognition>();

            services.AddDistributedDynamoDbCache(options =>
            {
                options.TableName = Environment.GetEnvironmentVariable("CACHE_TABLE_NAME");
                options.TtlAttribute = Environment.GetEnvironmentVariable("CACHE_TABLE_TTL_ATTRIBUTE_NAME");
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddRazorPages();

            IAmazonCognitoIdentityProvider identityProvider = new AmazonCognitoIdentityProviderClient();
            string userPoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID");
            string userPoolClientId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_CLIENT_ID");
            string userPoolClientSecret = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_CLIENT_SECRET");

            services.AddSingleton(identityProvider);
            services.AddSingleton(new CognitoUserPool(userPoolId, userPoolClientId, identityProvider, userPoolClientSecret));

            services.AddCognitoIdentity();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
