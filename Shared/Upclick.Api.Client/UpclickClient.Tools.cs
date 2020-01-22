using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;

namespace Upclick.Api.Client
{
    public partial class UpclickClient
    {
        private async Task<HttpResponseMessage> _XChangeRate(string isocode = null)
        {
            return await _upclickHttpClient.GetAsync("tools/xchangerate", string.IsNullOrEmpty(isocode) ? null : new NameValueCollection() { { "isocode", isocode } });
        }

        public async Task<List<XChange>> XChangeRate()
        {
            HttpResponseMessage response = await _XChangeRate();

            var json = await response.Content.ReadAsStringAsync();
            var root = await response.Content.ReadAsAsync<ToolsXChangeRateRoot>();
            if (!object.Equals(root, null) && !object.Equals(root.XChangeRate, null))
                return root.XChangeRate.XChanges;

            return null;
        }
    }

    public class ToolsXChangeRateRoot
    {
        [JsonProperty(PropertyName = "tools_xchangerate")]
        public ToolsXChangeRate XChangeRate { get; set; }
    }



    public class ToolsXChangeRate
    {
        [JsonProperty(PropertyName = "xchange")]
        [JsonConverter(typeof(SingleOrArrayConverter<XChange>))]
        public List<XChange> XChanges { get; set; }
    }

    public class XChange
    {
        [JsonProperty(PropertyName = "isoCode")]
        public string IsoCode { get; set; }

        [JsonProperty(PropertyName = "usRate")]
        public decimal UsRate { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "symbol")]
        public string Symbol { get; set; }
    }
}
