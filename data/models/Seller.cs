namespace PriceListApi.data.models
{
    public class Seller
    {
        public int SellerID { get; set; }
        public   required string SellerName { get; set; }
        public required string City { get; set; }
    }
}
