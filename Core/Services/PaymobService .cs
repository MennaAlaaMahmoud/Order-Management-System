using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServicesAbstractions;

namespace Services
{
    // الكريديت والمحفظه الالكترونيه
    public class PaymobService : IPaymobServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaymobService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> PayWithCardAsync(decimal amount, string email, string phone)
        {
            var apiKey = _config["Paymob:ApiKey"];
            var cardIntegrationId = _config["Paymob:CardIntegrationId"];
            var iframeId = _config["Paymob:IframeId"];

            // 1️⃣ Auth Token
            var authResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens",
                new StringContent(JsonConvert.SerializeObject(new { api_key = apiKey }), Encoding.UTF8, "application/json"));

            var authData = JsonConvert.DeserializeObject<dynamic>(await authResponse.Content.ReadAsStringAsync());
            string token = authData.token;

            // 2️⃣ Register Order
            var orderBody = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = (int)(amount * 100),
                currency = "EGP",
                items = new object[] { }
            };

            var orderResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders",
                new StringContent(JsonConvert.SerializeObject(orderBody), Encoding.UTF8, "application/json"));

            var orderData = JsonConvert.DeserializeObject<dynamic>(await orderResponse.Content.ReadAsStringAsync());
            int orderId = orderData.id;

            // 3️⃣ Payment Key
            var paymentKeyBody = new
            {
                auth_token = token,
                amount_cents = (int)(amount * 100),
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = email,
                    floor = "NA",
                    first_name = "User",
                    street = "NA",
                    building = "NA",
                    phone_number = phone,
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "Test",
                    state = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(cardIntegrationId)
            };

            var paymentKeyResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys",
                new StringContent(JsonConvert.SerializeObject(paymentKeyBody), Encoding.UTF8, "application/json"));

            var paymentKeyData = JsonConvert.DeserializeObject<dynamic>(await paymentKeyResponse.Content.ReadAsStringAsync());
            string paymentToken = paymentKeyData.token;

            // 4️⃣ Return Payment Iframe URL
            string iframeIdUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentToken}";
            return iframeIdUrl;
        }

        public async Task<string> PayWithWalletAsync(decimal amount, string phone)
        {
            var apiKey = _config["Paymob:ApiKey"];
            var walletIntegrationId = _config["Paymob:WalletIntegrationId"];

            // 1️⃣ Auth Token
            var authResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens",
                new StringContent(JsonConvert.SerializeObject(new { api_key = apiKey }), Encoding.UTF8, "application/json"));

            var authData = JsonConvert.DeserializeObject<dynamic>(await authResponse.Content.ReadAsStringAsync());
            string token = authData.token;

            // 2️⃣ Register Order
            var orderBody = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = (int)(amount * 100),
                currency = "EGP",
                items = new object[] { }
            };

            var orderResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders",
                new StringContent(JsonConvert.SerializeObject(orderBody), Encoding.UTF8, "application/json"));

            var orderData = JsonConvert.DeserializeObject<dynamic>(await orderResponse.Content.ReadAsStringAsync());
            int orderId = orderData.id;

            // 3️⃣ Payment Key
            var paymentKeyBody = new
            {
                auth_token = token,
                amount_cents = (int)(amount * 100),
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "NA",
                    email = "wallet@example.com",
                    floor = "NA",
                    first_name = "Wallet",
                    street = "NA",
                    building = "NA",
                    phone_number = phone,
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = "User",
                    state = "NA"
                },
                currency = "EGP",
                integration_id = int.Parse(walletIntegrationId)
            };

            var paymentKeyResponse = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys",
                new StringContent(JsonConvert.SerializeObject(paymentKeyBody), Encoding.UTF8, "application/json"));

            var paymentKeyData = JsonConvert.DeserializeObject<dynamic>(await paymentKeyResponse.Content.ReadAsStringAsync());
            string paymentToken = paymentKeyData.token;

            // 4️⃣ Wallet Request
            var walletInitBody = new
            {
                source = new
                {
                    identifier = phone,
                    subtype = "WALLET"
                },
                payment_token = paymentToken
            };

            var walletRequest = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payments/pay",
                new StringContent(JsonConvert.SerializeObject(walletInitBody), Encoding.UTF8, "application/json"));

            var walletResponseData = JsonConvert.DeserializeObject<dynamic>(await walletRequest.Content.ReadAsStringAsync());

            string redirectUrl = walletResponseData.redirect_url;
            return redirectUrl;
        }


    }
}
