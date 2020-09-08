using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Net5Example.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using System;

namespace BlazorApp1.Pages
{
    public partial class Products
    {
        [Inject]
        public IProductService ProductService { get; set; }

        private List<StockPosition> stockPositions;

        public int NumRows { get => stockPositions.Max(s => s.Row); }
        public int NumColumns { get => stockPositions.Max(s => s.Column); }

        public bool IsAllSync { get; set; }

        protected override async Task OnInitializedAsync()
        {
            stockPositions = await ProductService.GetStockPositions();

            SyncData();
        }

        protected async Task SelectProduct(int IdPositionStock)
        {
            var position = stockPositions.FirstOrDefault(s => s.StockPositionId == IdPositionStock);

            position.Quantity--;

            await ProductService.EditStockPosition(position);
        }

        async Task SyncData()
        {
            DateTime LNow = DateTime.Now;

            Console.WriteLine($"[{LNow.ToString("HH:mm:ss")}] Syncing...");

            IsAllSync = await ProductService.SyncData();

            Console.WriteLine($"[{LNow.ToString("HH:mm:ss")}] Synced?: " + IsAllSync);

            await Task.Delay(10000);
            
            SyncData();
        }
    }
}
