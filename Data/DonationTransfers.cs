namespace CoinSite.Data
{
    public class DonationTransfers
    {
        public Transaction[] result { get; set; }
    }

    public class Transaction
    {
        public string hash { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double value { get; set; }
    }
}
