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
    public class OrderItemsReserver : IOrderItemsReserver
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public OrderItemsReserver(string url)
        {
            _url = url;
            _httpClient = new HttpClient();
        }

        public async Task<bool> UploadOrderAsync(Order order)
        {
            var orderFile = new OrderFile
            {
                FileName = $"Order-{order.Id}.json",
                Order = order
            };

            var orderFileJson = JsonSerializer.Serialize(orderFile);

            var content = new StringContent(orderFileJson, Encoding.UTF8, "application/json");

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

    class OrderFile
    {
        public string FileName { get; set; }
        public Order Order { get; set; }
    }
}
