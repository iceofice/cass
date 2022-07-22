using System.ComponentModel.DataAnnotations;

namespace CASS___Construction_Assistance.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Constructor_Id { get; set; }
        public string ImageUrl { get; set; }
    }
}
