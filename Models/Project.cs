using System.ComponentModel.DataAnnotations;

namespace CASS___Construction_Assistance.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public string Location { get; set; }

        public string Status { get; set; }

        public string Constructor_Id { get; set; }

        public string ImageUrl { get; set; }
    }
}
