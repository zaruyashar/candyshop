using System.Collections.Generic;

namespace CANDYSHOP.Models
{
    public class HomeIndexViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Testimonial> Testimonials { get; set; } = new();
        public List<BlogPost> BlogPosts { get; set; } = new();
    }
}
