using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace AwsAspCore.DDB.Caching
{
    // https://stackoverflow.com/a/52752984/3159342
    public static class DynamoDBCacheServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Amazon DynamoDB caching services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{DynamoDbCacheOptions}"/> to configure the provided
        /// <see cref="DynamoDBCacheOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDistributedDynamoDbCache(this IServiceCollection services, Action<DynamoDBCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<IDistributedCache, DynamoDBCache>());

            return services;
        }
    }
}