namespace OnlineGallery.Models
{
    public class PaymentViewModel
    {
        public int ArtworkId { get; set; }
        public decimal Price { get; set; }
        public string Token { get; set; }
        public string PaymentPageUrl { get; set; }
    }
}
