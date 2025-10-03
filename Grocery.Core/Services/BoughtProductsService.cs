
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.Generic;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            var boughtProducts = new List<BoughtProducts>();

            if (productId == null)
            {
                return boughtProducts;
            }

            var groceryListItems = _groceryListItemsRepository.GetAll();

            foreach (var groceryListItem in groceryListItems)
            {
                if (groceryListItem.ProductId != productId)
                {
                    continue;
                }

                var product = _productRepository.Get(groceryListItem.ProductId);
                if (product == null)
                {
                    continue;
                }

                var groceryList = _groceryListRepository.Get(groceryListItem.GroceryListId);
                if (groceryList == null)
                {
                    continue;
                }

                var client = _clientRepository.Get(groceryList.ClientId);
                if (client == null)
                {
                    continue;
                }

                boughtProducts.Add(new BoughtProducts(client, groceryList, product));
            }

            return boughtProducts;
        }
    }
}
