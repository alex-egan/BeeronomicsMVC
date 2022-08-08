namespace BeeronomicsMVC.Models
{
    public class DrinkSimple
    {
        public DrinkSimple()
        {

        }
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string AddedBy { get; set; } = string.Empty;
        public decimal? ActivePrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public bool PriceLastIncreased { get; set; }
        public System.Timers.Timer Timer { get; set; }
    }
}
