using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cass.Models
{
    public class APIusers
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}

