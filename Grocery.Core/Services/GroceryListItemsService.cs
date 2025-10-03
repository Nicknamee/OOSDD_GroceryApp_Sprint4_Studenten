using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Linq;

namespace Grocery.Core.Services
{
	public sealed class GroceryListItemsService : IGroceryListItemsService
	{
		private readonly IGroceryListItemsRepository _groceriesRepository;
		private readonly IProductRepository _productRepository;

		public GroceryListItemsService(
			IGroceryListItemsRepository groceriesRepository,
			IProductRepository productRepository)
		{
			_groceriesRepository = groceriesRepository ?? throw new ArgumentNullException(nameof(groceriesRepository));
			_productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
		}

		public List<GroceryListItem> GetAll()
		{
			var items = _groceriesRepository.GetAll();
			PopulateProducts(items);
			return items;
		}

		public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
		{
			var items = _groceriesRepository
				.GetAll()
				.Where(g => g.GroceryListId == groceryListId)
				.ToList();

			PopulateProducts(items);
			return items;
		}

		public GroceryListItem Add(GroceryListItem item) => _groceriesRepository.Add(item);

		public GroceryListItem? Delete(GroceryListItem item)
		{
		
			throw new NotImplementedException();
		}

		public GroceryListItem? Get(int id) => _groceriesRepository.Get(id);

		public GroceryListItem? Update(GroceryListItem item) => _groceriesRepository.Update(item);

	
		public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
		{
			if (topX <= 0) return new();

			var groceryListItems = _groceriesRepository.GetAll();
			if (groceryListItems.Count == 0) return new();

			var totals = groceryListItems
				.GroupBy(i => i.ProductId)
				.Select(g => new { ProductId = g.Key, Total = g.Sum(i => i.Amount) })
				.OrderByDescending(x => x.Total)
				.Take(topX)
				.ToList();

		
			var productIds = totals.Select(t => t.ProductId).Distinct().ToList();
			var productMap = new Dictionary<int, Product>();
			foreach (var id in productIds)
			{
			
				productMap[id] = _productRepository.Get(id) ?? new Product(0, string.Empty, 0);
			}

			var result = new List<BestSellingProducts>(totals.Count);
			var rank = 0;
			foreach (var t in totals)
			{
				rank++;
				var p = productMap[t.ProductId];
				result.Add(new BestSellingProducts(t.ProductId, p.Name, p.Stock, t.Total, rank));
			}

			return result;
		}

		
		private void PopulateProducts(List<GroceryListItem> items)
		{
			if (items == null || items.Count == 0) return;

			var ids = items.Select(i => i.ProductId).Distinct().ToList();
			var productMap = new Dictionary<int, Product>();

			foreach (var id in ids)
			{
				productMap[id] = _productRepository.Get(id) ?? new Product(0, string.Empty, 0);
			}

			foreach (var g in items)
			{
				g.Product = productMap[g.ProductId];
			}
		}
	}
}
