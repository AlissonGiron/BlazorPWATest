
namespace Net5Example.ViewModels
{
    public class StockPosition
    {
        public int StockPositionId { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
