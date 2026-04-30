namespace TourPlanner.Models
{
    public class Tour
    {
        public required string Id {get; set; }
        public required string userId{get; set;}
        public string title {get; set;}
        public string tourDescription{get; set;}
        public required string from {get; set;}
        public required string to {get; set;}
        public string transportType{get; set;}
    }
}