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
        private async Task<HttpResponseMessage> _GetCrossells(string uid = null)
        {
            return await _upclickHttpClient.GetAsync("crossells/retrieve", string.IsNullOrEmpty(uid) ? null : new NameValueCollection() { { "uid", uid } });
        }

        private async Task<HttpResponseMessage> _GetCrossellsList(string uid = null)
        {
            return await _upclickHttpClient.GetAsync("crossells/getlist", string.IsNullOrEmpty(uid) ? null : new NameValueCollection() { { "uid", uid } });
        }

        public async Task<List<Crossell>> GetCrossells()
        {
            HttpResponseMessage response = await _GetCrossells();

            var root = await response.Content.ReadAsAsync<CrossellsRetrieveRoot>();
            if (!object.Equals(root, null) && !object.Equals(root.Crossells, null))
                return root.Crossells.Products;

            return null;
        }

        public async Task<List<Crossell>> GetCrossellsList()
        {
            HttpResponseMessage response = await _GetCrossellsList();

            var root = await response.Content.ReadAsAsync<CrossellsGetListRoot>();
            if (!object.Equals(root, null) && !object.Equals(root.CrossellsGetList, null))
                return root.CrossellsGetList.Products;

            return null;
        }
    }

    public class CrossellsRetrieveRoot
    {
        [JsonProperty(PropertyName = "crossells_retrieve")]
        public Crossells Crossells { get; set; }
    }

    public class CrossellsGetListRoot
    {
        [JsonProperty(PropertyName = "crossells_getlist")]
        public CrossellsGetList CrossellsGetList { get; set; }
    }

    public class CrossellsGetList
    {
        [JsonProperty(PropertyName = "crossell")]
        [JsonConverter(typeof(SingleOrArrayConverter<Crossell>))]
        public List<Crossell> Products { get; set; }
    }

    public class Crossells
    {
        [JsonProperty(PropertyName = "crossell")]
        [JsonConverter(typeof(SingleOrArrayConverter<Crossell>))]
        public List<Crossell> Products { get; set; }
    }

    public class Crossell : Product
    {
        public string BaseproductUid { get; set; }
    }
}
