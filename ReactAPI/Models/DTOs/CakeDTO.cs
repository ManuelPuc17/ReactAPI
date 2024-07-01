namespace ReactAPI.Models.DTOs
{
    public class CakeDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Origin { get; set; }

        public decimal Price { get; set; }

        public double AverageFlavor { get; set; }

        public double AveragePresentation { get; set; }

        public double Finalaverage { get; set; }
        public string? ImageUrl { get; set; }
    }
}
