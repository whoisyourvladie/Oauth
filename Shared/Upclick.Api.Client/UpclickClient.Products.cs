using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace Upclick.Api.Client
{
    public partial class UpclickClient
    {
        private async Task<HttpResponseMessage> _GetProducts(string uid = null)
        {
            return await _upclickHttpClient.GetAsync("products/retrieve", string.IsNullOrEmpty(uid) ? null : new NameValueCollection() { { "uid", uid } });
        }

        private async Task<HttpResponseMessage> _GetProductsList(string uid = null)
        {
            return await _upclickHttpClient.GetAsync("products/getlist", string.IsNullOrEmpty(uid) ? null : new NameValueCollection() { { "uid", uid } });
        }

        public async Task<List<Product>> GetProducts()
        {
            HttpResponseMessage response = await _GetProducts();

            var root = await response.Content.ReadAsAsync<ProductsRetrieveRoot>();
            if (!object.Equals(root, null) && !object.Equals(root.ProductsRetrieve, null))
                return root.ProductsRetrieve.Products;

            return null;
        }

        public async Task<List<Product>> GetProductsList()
        {
            HttpResponseMessage response = await _GetProductsList();

            var root = await response.Content.ReadAsAsync<ProductsGetListRoot>();
            if (!object.Equals(root, null) && !object.Equals(root.ProductsGetList, null))
                return root.ProductsGetList.Products;

            return null;
        }
    }

    public class ProductsRetrieveRoot
    {
        [JsonProperty(PropertyName = "products_retrieve")]
        public ProductsRetrieve ProductsRetrieve { get; set; }
    }

    public class ProductsRetrieve
    {
        [JsonProperty(PropertyName = "product")]
        [JsonConverter(typeof(SingleOrArrayConverter<Product>))]
        public List<Product> Products { get; set; }
    }

    public class ProductsGetListRoot
    {
        [JsonProperty(PropertyName = "products_getlist")]
        public ProductsGetList ProductsGetList { get; set; }
    }

    public class ProductsGetList
    {
        [JsonProperty(PropertyName = "product")]
        [JsonConverter(typeof(SingleOrArrayConverter<Product>))]
        public List<Product> Products { get; set; }
    }

    public class Product
    {
        public string Uid { get; set; }

        public string ProductTitle { get; set; }

        [JsonProperty(PropertyName = "names")]
        public ProductNames ProductNames { get; set; }
    }

    public class ProductNames
    {
        [JsonProperty("name")]
        [JsonConverter(typeof(SingleOrArrayConverter<ProductName>))]
        public List<ProductName> Names { get; set; }
    }

    [JsonObject("Name")]
    public class ProductName
    {
        [JsonProperty("culture")]
        public string Cultute { get; set; }

        [JsonProperty("producttitle")]
        public string ProductTitle { get; set; }
    }

    class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
                return token.ToObject<List<T>>();

            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
