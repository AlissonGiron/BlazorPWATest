using Net5Example.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp1.Services
{
    public interface IProductService
    {
        public Task<List<StockPosition>> GetStockPositions();
        public Task EditStockPosition(StockPosition APosition, bool AFromSync = false);
        Task<bool> SyncData();
    }
}
