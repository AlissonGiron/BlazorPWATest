using BlazorApp1.Pages;
using Blazored.LocalStorage;
using Net5Example.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public class ProductService: IProductService
    {
        HttpClient FClient;
        ILocalStorageService FLocalStorage;

        public ProductService(HttpClient AClient, ILocalStorageService ALocalStorage)
        {
            FClient = AClient;
            FLocalStorage = ALocalStorage;
        }

        public async Task<bool> SyncData()
        {
            Console.WriteLine("Getting Pending List");
            
            List<int> LPendingEdit = await FLocalStorage.GetItemAsync<List<int>>("Pending");

            Console.WriteLine("Pending Count: " + LPendingEdit.Count);

            if (!LPendingEdit.Any())
            {
                Console.WriteLine("All Synced");
                return true;
            }

            Console.WriteLine("Not Synced");
            foreach (int LPendingProduct in LPendingEdit)
            {
                Console.WriteLine("Trying to Sync " + LPendingProduct);

                await EditStockPosition(await FLocalStorage.GetItemAsync<StockPosition>(LPendingProduct.ToString()), true);
            }

            LPendingEdit = await FLocalStorage.GetItemAsync<List<int>>("Pending");

            return !LPendingEdit.Any();
        }

        public async Task EditStockPosition(StockPosition APosition, bool AFromSync = false)
        {
            if(!AFromSync)
            {
                await FLocalStorage.SetItemAsync(APosition.StockPositionId.ToString(), APosition);
            }

            try
            {
                Console.WriteLine("Sending Request...");

                HttpResponseMessage LResult = await FClient.PutAsJsonAsync("http://localhost:8085/api/stockposition/" + APosition.StockPositionId, APosition);

                Console.WriteLine("Request finished");

                if (AFromSync)
                {
                    List<int> LPendingEdit = await FLocalStorage.GetItemAsync<List<int>>("Pending");
                    LPendingEdit.RemoveAll(s => s == APosition.StockPositionId);
                    await FLocalStorage.SetItemAsync("Pending", LPendingEdit);
                }

                LResult.EnsureSuccessStatusCode();

                Console.WriteLine("Request Ok");
            }
            catch
            {
                Console.WriteLine("Request NOk, saving in pendind list");

                List<int> LPendingEdit = await FLocalStorage.GetItemAsync<List<int>>("Pending");
                bool LAddToPending = false;

                if(LPendingEdit == null)
                {
                    LPendingEdit = new List<int>() { APosition.StockPositionId };
                    LAddToPending = true;
                }
                else if(!LPendingEdit.Contains(APosition.StockPositionId))
                {
                    LPendingEdit.Add(APosition.StockPositionId);
                    LAddToPending = true;
                }

                if(LAddToPending)
                {
                    await FLocalStorage.SetItemAsync("Pending", LPendingEdit);
                }
            }
        }

        public async Task<List<StockPosition>> GetStockPositions()
        {
            List<StockPosition> LPositions = new List<StockPosition>();

            List<int> LPendingEdit = await FLocalStorage.GetItemAsync<List<int>>("Pending");

            if(LPendingEdit.Any())
            {
                Console.WriteLine("Pending Items found, syncing...");
                await SyncData();
            }

            try
            {
                Console.WriteLine("Read from server...");

                LPositions = await FClient.GetFromJsonAsync<List<StockPosition>>("http://localhost:8085/api/stockposition");

                Console.WriteLine("Read Ok");

                foreach (StockPosition LPosition in LPositions)
                {
                    await FLocalStorage.SetItemAsync(LPosition.StockPositionId.ToString(), LPosition);
                }
            }
            catch
            {
                Console.WriteLine("Read NOk, reading from local");

                int LCount = await FLocalStorage.LengthAsync();

                for (int i = 0; i < LCount; i++)
                {
                    string LKey = await FLocalStorage.KeyAsync(i);

                    if(LKey != "Pending")
                    {
                        LPositions.Add(await FLocalStorage.GetItemAsync<StockPosition>(LKey));
                    }
                }
            }

            return LPositions;
        }
    }
}