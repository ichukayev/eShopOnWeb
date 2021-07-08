using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public OrderProcessor(string url)
        {
            _url = url;
            _httpClient = new HttpClient();
        }

        public async Task<bool> UploadOrderToOrderProcessorAsync(Order order)
        {
            var orderJson = JsonSerializer.Serialize(order);

            var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

            using (var message = await _httpClient.PostAsync(_url, content))
            {
                if (!message.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            return true;

        }
    }
}
