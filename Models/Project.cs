using cass.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace cass.Models
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

        public string Constructor_Name { get; set; }

        public string Customer_Id { get; set; }

        public string Customer_Name { get; set; }

        public string ImageUrl { get; set; }
    }
}
