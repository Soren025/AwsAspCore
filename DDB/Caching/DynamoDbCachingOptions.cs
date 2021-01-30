using Microsoft.Extensions.Options;

using System;

namespace AwsAspCore.DDB.Caching
{
    // https://stackoverflow.com/a/52752984/3159342
    public class DynamoDBCacheOptions : IOptions<DynamoDBCacheOptions>
    {
        public string TableName { get; set; } = "ASP.NET_SessionState";

        public TimeSpan IdleTimeout { get; set; } = new TimeSpan(0, 20, 0);

        public string TtlAttribute { get; set; } = "TTL";

        DynamoDBCacheOptions IOptions<DynamoDBCacheOptions>.Value
        {
            get { return this; }
        }
    }
}