using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Upclick.Api.Client
{
    public partial class UpclickClient
    {
        private async Task<CustomerSubscription> ReadCustomerSubscription(HttpResponseMessage response, string xmlRoot)
        {
            if (!response.IsSuccessStatusCode)
                return null;

            var jObject = await response.Content.ReadAsAsync<JObject>();

            if (!object.Equals(jObject, null))
            {
                var token = jObject.SelectToken(xmlRoot);

                if (!object.Equals(token, null))
                    return token.ToObject<CustomerSubscription>();
            }

            return null;
        }

        private async Task<HttpResponseMessage> _SubscriptionSuspendResume(string id, bool suspend, string source = null)
        {
            var url = suspend ? "customers/subscriptions/do/suspend" : "customers/subscriptions/do/resume";
            var param = new NameValueCollection() { { "id", id } };
            if (!String.IsNullOrEmpty(source))
            {
                param.Add("source", source);
            }

            return await _upclickHttpClient.GetAsync(url, param);
        }
        private async Task<CustomersSubscription> _SubscriptionSuspendResume(string id, bool suspend, string key, string source = null)
        {
            HttpResponseMessage response = await _SubscriptionSuspendResume(id, suspend, source);

            if (!response.IsSuccessStatusCode)
                return null;

            var jObject = await response.Content.ReadAsAsync<JObject>();

            if (!object.Equals(jObject, null))
            {
                var token = jObject.SelectToken(key);

                if (!object.Equals(token, null))
                {
                    if (token is JArray)
                    {
                        var tokenArray = (JArray)token;
                        var tokens = tokenArray.ToObject<CustomersSubscription[]>();

                        return tokens.OrderByDescending(e => e.NextRebillDate).FirstOrDefault();
                    }

                    return token.ToObject<CustomersSubscription>();
                }
            }

            return null;
        }
        private async Task<HttpResponseMessage> _SubscriptionCancel(string id)
        {
            return await _upclickHttpClient.GetAsync("customers/subscriptions/do/cancel", new NameValueCollection() { { "id", id } });
        }
        private async Task<HttpResponseMessage> _SubscriptionDetails(string id)
        {
            return await _upclickHttpClient.GetAsync("customers/subscriptions/retrieve/subscriptiondetails", new NameValueCollection() { { "id", id } });
        }
        private async Task<HttpResponseMessage> _SubscriptionUpdate(string id, DateTime nextRebillDate)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query.Add("id", id);
            query.Add("rebillDate", nextRebillDate.ToString("yyyy-MM-dd"));

            return await _upclickHttpClient.GetAsync("customers/subscriptions/do/update", query);
        }

        public async Task<CustomersSubscription> SubscriptionSuspend(string id, string source=null)
        {
            return await _SubscriptionSuspendResume(id:id, suspend:true, key:"$.customers_subscriptions_do_suspend.subscription",source:source);
        }
        public async Task<CustomersSubscription> SubscriptionResume(string id)
        {
            return await _SubscriptionSuspendResume(id:id, suspend:false, key:"$.customers_subscriptions_do_resume.subscription");
        }
        public async Task<CustomerSubscription> SubscriptionCancel(string id)
        {
            HttpResponseMessage response = await _SubscriptionCancel(id);

            if (!response.IsSuccessStatusCode)
                return null;

            var jObject = await response.Content.ReadAsAsync<JObject>();

            if (!object.Equals(jObject, null))
            {
                var token = jObject.SelectToken("$.customers_subscriptions_do_cancel.result");

                if (!object.Equals(token, null))
                    return token.ToObject<CustomerSubscription>();
            }

            return null;
        }
        public async Task<CustomerSubscription> SubscriptionDetails(string id)
        {
            var response = await _SubscriptionDetails(id);

            return await ReadCustomerSubscription(response, "$.customers_subscriptions_retrieve_subscriptiondetails.subscription");
        }
        public async Task<CustomerSubscription> SubscriptionUpdate(string id, DateTime nextRebillDate)
        {
            var response = await _SubscriptionUpdate(id, nextRebillDate);

            return await ReadCustomerSubscription(response, "$.customers_subscriptions_do_update.subscription");
        }

        private async Task<HttpResponseMessage> _SubscriptionsPaymentInstruments(string email)
        {
            return await _upclickHttpClient.GetAsync("customers/subscriptions/paymentinstruments", new NameValueCollection() { { "email", email } });
        }
        public async Task<CustomerPaymentInstrument[]> SubscriptionsPaymentInstruments(string email)
        {
            var response = await _SubscriptionsPaymentInstruments(email);

            if (!response.IsSuccessStatusCode)
                return null;

            var jObject = await response.Content.ReadAsAsync<JObject>();

            if (!object.Equals(jObject, null))
            {
                var instruments = jObject.SelectToken("$.customers_subscriptions_paymentinstruments.paymentinstruments");

                if (!object.Equals(instruments, null))
                {
                    if (instruments is JArray)
                    {
                        var instrumentsArray = (JArray)instruments;
                        return instrumentsArray.ToObject<CustomerPaymentInstrument[]>();
                    }

                    return new[] { instruments.ToObject<CustomerPaymentInstrument>() };
                }
            }

            return null;
        }
    }

    public class CustomersSubscription
    {
        public string Status { get; set; }
        public DateTime NextRebillDate { get; set; }
    }
    public class CustomerSubscription
    {
        public CustomerSubscriptionStatus Status { get; set; }
        public CustomerSubscriptionNextCycleBill NextCycleBill { get; set; }
    }
    public class CustomerSubscriptionNextCycleBill
    {
        public DateTime Date { get; set; }
    }
    public class CustomerSubscriptionStatus
    {
        public string Name { get; set; }
    }
    public class CustomerPaymentInstrument
    {
        public string PayTokenID { get; set; }
        public string ProductName { get; set; }
        public string PayType { get; set; }
        public string MaskedAccountNumber { get; set; }
        public uint Expiration { get; set; }
        public string EditUrl { get; set; }
    }
}