namespace CoinSite.Data
{
    public class DonationTransfers
    {
        public Transaction[] result { get; set; }
    }

    public class Transaction
    {
        public string hash { get; set; }
        public string from_address { get; set; }
        public string to_address { get; set; }
        public double value { get; set; }
        public DateTime block_timestamp { get; set; }
    }
}
