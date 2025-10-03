using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Enums;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class GroceryListViewModel : BaseViewModel
    {
        public ObservableCollection<GroceryList> GroceryLists { get; set; }
        private readonly IGroceryListService _groceryListService;
        private readonly GlobalViewModel _global;

        [ObservableProperty]
        private Client? client;

        public GroceryListViewModel(IGroceryListService groceryListService, GlobalViewModel globalViewModel)
        {
            Title = "Boodschappenlijst";
            _groceryListService = groceryListService;
            _global = globalViewModel;
            GroceryLists = new(_groceryListService.GetAll());
        }

        [RelayCommand]
        public async Task SelectGroceryList(GroceryList groceryList)
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), groceryList } };
            await Shell.Current.GoToAsync($"{nameof(Views.GroceryListItemsView)}?Titel={groceryList.Name}", true, paramater);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            Client = _global.Client;
            GroceryLists = new(_groceryListService.GetAll());
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            GroceryLists.Clear();
        }

        [RelayCommand]
        private async Task ShowBoughtProducts()
        {
            if (Client?.Role != Role.Admin)
            {
                return;
            }

            await Shell.Current.GoToAsync(nameof(Views.BoughtProductsView));
        }
    }
}
