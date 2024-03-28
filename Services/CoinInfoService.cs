using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CoinSite.Data;

namespace CoinSite.Services
{
    public class CoinInfoService
    {
        //ADDRESSES
        protected static string coinAddress = "0xA1e800791EE8b80665678B69C3d1b0Fd4D4B5E23";
        protected static string donationWalletAddress = "0xae5E588beea22416c4827126EAAA4fdB892F4f01";
        protected static string saveTheChildrenAddress = "0x290191681a2d043C7177471F763549cc58404caF";
        protected static string donorsChooseAddress = "0x7D2869e7266d3eC5614FE0C073B93be67b4bd356";
        protected static string scholarshipAddress = "0xc48ef5b8D9A4fC9BE4C39A319658eFFaBF6cCa61";

        //API KEYS
        protected static string etherscanAPIKey = "RGHQ3G5IH3FS2BV3XVTYXVKWYN9SURDBAA";
        protected static string moralisAPIKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJub25jZSI6IjAyZmI3NzNkLWFjNTQtNDIyNS1iYjAwLTBiMGRhYjgzMjJkNyIsIm9yZ0lkIjoiMzgzMzIxIiwidXNlcklkIjoiMzkzODYzIiwidHlwZUlkIjoiN2FhMzNlZTktZjc0Ni00ZDA0LTlhZTItMTk3NGZkMDY3NDRiIiwidHlwZSI6IlBST0pFQ1QiLCJpYXQiOjE3MTA2OTc2NTAsImV4cCI6NDg2NjQ1NzY1MH0.-h3skKNRQjdJE3eVubSrQy9GpPCfuaqHk031VwfqPA0";

        //TRANSACTION INFO
        protected List<Transaction> transactions = new List<Transaction>();

        //COIN PRICES
        protected Tuple<double, bool> classcoinPrice;

        public CoinInfoService()
        {
            transactions = GetDonationTransfers();           
        }

        public List<Transaction> GetDonationTransfers()
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri(@"https://api.etherscan.io/") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("X-API-Key", moralisAPIKey);
            var httpResponse = client.GetAsync("api?module=account&action=txlist&address=" + donationWalletAddress + "&startblock=19512236&endblock=99999999&apikey=" + etherscanAPIKey).Result;
            List<Transaction> donationTransfers = new List<Transaction>();

            if (httpResponse.IsSuccessStatusCode)
            {
                string result = httpResponse.Content.ReadAsStringAsync().Result;
                DonationTransfers donations = JsonConvert.DeserializeObject<DonationTransfers>(result);
                donationTransfers = donations == null ? new List<Transaction>() : donations.result.ToList();
            }

            return donationTransfers;
        }

        public double GetEthereumPrice()
        {
            double etherprice = 0.0;
            HttpClient client = new HttpClient { BaseAddress = new Uri(@"https://api.coinbase.com/") };
            var httpResponse = client.GetAsync("v2/exchange-rates?currency=ETH").Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                string result = httpResponse.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject(result);
                JObject data = JObject.Parse(obj.ToString());
                etherprice = Convert.ToDouble(((data["data"])["rates"])["USD"].ToString());
            }

            return etherprice;
        }

        //positve 24 hr percent change makes the price green -> Tuple(price, positive 24hr change)
        public void SetClassCoinPrice()
        {
            double tokenPrice = 0.0;
            bool positiveChange = true;

            HttpClient client = new HttpClient { BaseAddress = new Uri(@"https://deep-index.moralis.io/") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-API-Key", moralisAPIKey);
            var httpResponse = client.GetAsync("api/v2.2/erc20/" + coinAddress + "/price?chain=eth&include=percent_change").Result;

            if (httpResponse.IsSuccessStatusCode)
            {
                string result = httpResponse.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject(result);
                JObject data = JObject.Parse(obj.ToString());
                tokenPrice = Convert.ToDouble(data["usdPrice"].ToString());
                positiveChange = Convert.ToDouble(data["24hrPercentChange"].ToString()) > 0;
            }

            classcoinPrice = new Tuple<double, bool>(tokenPrice, positiveChange);
        }

        public double GetEthereumRaised()
        {
            double raisedAmount = GetScholarShipAmericaRaised() + GetSaveTheChildrenRaised() + GetDonorsChooseRaised();

            return raisedAmount;
        }

        public double GetSaveTheChildrenRaised()
        {
            double totals = transactions.Where(val => val.to == saveTheChildrenAddress.ToLower()).Sum(val => (val.value * 0.000000000000000001));
            return totals;
        }

        public double GetDonorsChooseRaised()
        {
            double totals = transactions.Where(val => val.to == donorsChooseAddress.ToLower()).Sum(val => (val.value * 0.000000000000000001));
            return totals;
        }

        public double GetScholarShipAmericaRaised()
        {
            double totals = transactions.Where(val => val.to == scholarshipAddress.ToLower()).Sum(val => (val.value * 0.000000000000000001));
            return totals;
        }

        public Tuple<double, bool> GetClassCoinPrice()
        {
            return classcoinPrice;
        }
    }
}
