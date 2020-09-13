using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;

using AwsAspCore.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsAspCore.DDB
{
    public static class DDBExtensions
    {
        private static class TableNames
        {
            private const string EnvironmentVarriable_MoviesTableName = "MOVIES_TABLE_NAME";

            public static readonly string Movies = "movies";

            static TableNames()
            {
                Movies = Environment.GetEnvironmentVariable(EnvironmentVarriable_MoviesTableName) ?? Movies;
            }
        }

        public static class IndexNames
        {
            public const string Movies_OrderedByTitle = "movies-orderedByTitle";
        }

        public static class AttributeNames
        {
            public const string Movies_Id = "id";
            public const string Movies_Title = "title";
            public const string Movies_ReleaseDate = "releaseDate";
            public const string Movies_Genre = "genre";
            public const string Movies_Price = "price";
        }

        public static class AttributeValues
        {
            public static ValueConverter<string> StringValue { get; } = new StringValueConverter();

            public static ValueConverter<int> IntValue { get; } = new IntValueConverter();

            public static ValueConverter<decimal> DecimalValue { get; } = new DecimalValueConverter();

            public static ValueConverter<Guid> GuidValue { get; } = new GuidValueConverter();

            public static ValueConverter<DateTime> DateTimeValue { get; } = new DateTimeValueConverter();

            public abstract class ValueConverter<T>
            {
                public abstract AttributeValue Of(T value);

                public abstract T Get(AttributeValue value);

                public T GetOrDefault(Dictionary<string, AttributeValue> attributes, string name, T def = default) => attributes.TryGetValue(name, out var value) ? Get(value) : def;
            }

            private class StringValueConverter : ValueConverter<string>
            {
                public override AttributeValue Of(string value) => new AttributeValue { S = value };

                public override string Get(AttributeValue value) => value.S;
            }

            private class IntValueConverter : ValueConverter<int>
            {
                public override AttributeValue Of(int value) => new AttributeValue { N = value.ToString() };

                public override int Get(AttributeValue value) => int.Parse(value.N);
            }

            private class DecimalValueConverter : ValueConverter<decimal>
            {
                public override AttributeValue Of(decimal value) => new AttributeValue { N = value.ToString() };

                public override decimal Get(AttributeValue value) => decimal.Parse(value.N);
            }

            private class GuidValueConverter : ValueConverter<Guid>
            {
                public override AttributeValue Of(Guid value) => new AttributeValue { S = value.ToString() };

                public override Guid Get(AttributeValue value) => Guid.Parse(value.S);
            }

            private class DateTimeValueConverter : ValueConverter<DateTime>
            {
                public override AttributeValue Of(DateTime value) => new AttributeValue { S = value.ToString() };

                public override DateTime Get(AttributeValue value) => DateTime.Parse(value.S);
            }
        }

        public static class Keys
        {
            public static Dictionary<string, AttributeValue> Movies(Guid id) => new Dictionary<string, AttributeValue>
            {
                { AttributeNames.Movies_Id, AttributeValues.GuidValue.Of(id) },
            };
        }

        public static async Task<List<Movie>> Movies_GetAll<TSortKey>(this IAmazonDynamoDB ddb, Func<Movie, TSortKey> keyFunction, IComparer<TSortKey> comparer = null)
        {
            var response = await ddb.ScanAsync(new ScanRequest
            {
                TableName = TableNames.Movies,
                IndexName = IndexNames.Movies_OrderedByTitle,
            });

            return response.Items.Select(item => new Movie
            {
                ID = AttributeValues.GuidValue.GetOrDefault(item, AttributeNames.Movies_Id),
                Title = AttributeValues.StringValue.GetOrDefault(item, AttributeNames.Movies_Title),
                ReleaseDate = AttributeValues.DateTimeValue.GetOrDefault(item, AttributeNames.Movies_ReleaseDate),
                Genre = AttributeValues.StringValue.GetOrDefault(item, AttributeNames.Movies_Genre),
                Price = AttributeValues.DecimalValue.GetOrDefault(item, AttributeNames.Movies_Price),
            })
            .OrderBy(keyFunction, comparer)
            .ToList();
        }

        public static async Task<int> Movies_Count(this IAmazonDynamoDB ddb)
        {
            var response = await ddb.ScanAsync(new ScanRequest
            {
                TableName = TableNames.Movies,
                IndexName = IndexNames.Movies_OrderedByTitle,
                Select = Select.COUNT,
            });

            return response.Count;
        }

        public static async Task<Movie> Movies_Get(this IAmazonDynamoDB ddb, Guid id)
        {
            var response = await ddb.GetItemAsync(new GetItemRequest
            {
                TableName = TableNames.Movies,
                Key = Keys.Movies(id),
                ConsistentRead = true,
            });

            return response.IsItemSet ? new Movie
            {
                ID = AttributeValues.GuidValue.GetOrDefault(response.Item, AttributeNames.Movies_Id),
                Title = AttributeValues.StringValue.GetOrDefault(response.Item, AttributeNames.Movies_Title),
                ReleaseDate = AttributeValues.DateTimeValue.GetOrDefault(response.Item, AttributeNames.Movies_ReleaseDate),
                Genre = AttributeValues.StringValue.GetOrDefault(response.Item, AttributeNames.Movies_Genre),
                Price = AttributeValues.DecimalValue.GetOrDefault(response.Item, AttributeNames.Movies_Price),
            } : null;
        }

        public static async Task Movies_Update(this IAmazonDynamoDB ddb, Movie movie)
        {
            LambdaLogger.Log("MOVIE_UPDATE: " + movie.ID);

            await ddb.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = TableNames.Movies,
                Key = Keys.Movies(movie.ID),
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#title", AttributeNames.Movies_Title },
                    { "#releaseDate", AttributeNames.Movies_ReleaseDate },
                    { "#genre", AttributeNames.Movies_Genre },
                    { "#price", AttributeNames.Movies_Price },
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":title", AttributeValues.StringValue.Of(movie.Title) },
                    { ":releaseDate", AttributeValues.DateTimeValue.Of(movie.ReleaseDate) },
                    { ":genre", AttributeValues.StringValue.Of(movie.Genre) },
                    { ":price", AttributeValues.DecimalValue.Of(movie.Price) },
                },
                UpdateExpression = "SET #title = :title, #releaseDate = :releaseDate, #genre = :genre, #price = :price"
            });
        }

        public static async Task Movies_Remove(this IAmazonDynamoDB ddb, Guid id)
        {
            await ddb.DeleteItemAsync(new DeleteItemRequest
            {
                TableName = TableNames.Movies,
                Key = Keys.Movies(id),
            });
        }
    }
}
