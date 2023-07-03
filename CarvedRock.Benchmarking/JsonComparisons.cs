using BenchmarkDotNet.Attributes;
using CarvedRock.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarvedRock.Benchmarking
{
    public class JsonComparisons
    {
        private readonly List<ProductModel> _products;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonComparisons()
        {
            _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            _products = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = 1, Name = "First Product", Category = "boots",
                    Description =" sdfgsdfg sdfgdf",ImgUrl = "https://someurl/firstimage",
                    NumberOfRatings = 5, Price = 20.99, Rating = 4.2M
                },
                new ProductModel
                {
                    Id = 2, Name = "Second Product", Category = "boots",
                    Description =" sdfgsdfg sdfgdf",ImgUrl = "https://someurl/secondimage",
                    NumberOfRatings = 5, Price = 20.99, Rating = 4.2M
                },
                new ProductModel
                {
                    Id = 3, Name = "Third Product", Category = "boots",
                    Description =" sdfgsdfg sdfgdf",ImgUrl = "https://someurl/thirdimage",
                    NumberOfRatings = 5, Price = 20.99, Rating = 4.2M
                }
            };
        }

        [Benchmark]
        public string Newtonsoft() => JsonConvert.SerializeObject(_products);

        [Benchmark]
        public string SystemTextJson() => System.Text.Json.JsonSerializer.Serialize(_products, _jsonOptions);

        [Benchmark]
        public string SourceGenerator() => System.Text.Json.JsonSerializer.Serialize(_products,
            SourceGenerationContext.Default.ListProductModel);
    }

}
