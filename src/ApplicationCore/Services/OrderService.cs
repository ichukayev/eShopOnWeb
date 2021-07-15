﻿using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
<<<<<<< Updated upstream
=======
        private readonly IOrderItemsReserver _orderItemsReserver;
        private readonly IOrderProcessor _orderProcessor;
        private readonly IServiceBusService _serviceBusService;
>>>>>>> Stashed changes

        public OrderService(IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository,
<<<<<<< Updated upstream
            IUriComposer uriComposer)
        {
            _orderRepository = orderRepository;
            _uriComposer = uriComposer;
=======
            IUriComposer uriComposer,
            IOrderItemsReserver orderItemsReserver,
            IOrderProcessor orderProcessor,
            IServiceBusService serviceBusService)
        {
            _orderRepository = orderRepository;
            _uriComposer = uriComposer;
            _orderItemsReserver = orderItemsReserver;
            _orderProcessor = orderProcessor;
            _serviceBusService = serviceBusService;
>>>>>>> Stashed changes
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
        }

        public async Task CreateOrderAsync(int basketId, Address shippingAddress)
        {
            var basketSpec = new BasketWithItemsSpecification(basketId);
            var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

            Guard.Against.NullBasket(basketId, basket);
            Guard.Against.EmptyBasketOnCheckout(basket.Items);

            var catalogItemsSpecification = new CatalogItemsSpecification(basket.Items.Select(item => item.CatalogItemId).ToArray());
            var catalogItems = await _itemRepository.ListAsync(catalogItemsSpecification);

            var items = basket.Items.Select(basketItem =>
            {
                var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _uriComposer.ComposePicUri(catalogItem.PictureUri));
                var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);
                return orderItem;
            }).ToList();

            var order = new Order(basket.BuyerId, shippingAddress, items);

            await _orderRepository.AddAsync(order);
<<<<<<< Updated upstream
=======

            //await _orderProcessor.UploadOrderToOrderProcessorAsync(order);

            var orderJson = JsonConvert.SerializeObject(order);

            await _serviceBusService.SendSalesMessageAsync(orderJson);
>>>>>>> Stashed changes
        }
    }
}
