namespace ReactAPI.Models.DTOs
{
    public class RatingDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? CakeName { get; set; }

        public int CakeId { get; set; }

        public double Flavor { get; set; }

        public double Presentation { get; set; }

        public string? CakeImageUrl { get; set; }
    }
}
