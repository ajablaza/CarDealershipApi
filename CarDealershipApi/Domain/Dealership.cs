namespace CarDealershipApi.Domain
{
    public class Dealership
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}