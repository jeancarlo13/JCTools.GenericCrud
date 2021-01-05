using System.ComponentModel.DataAnnotations;

namespace Test5._0.Models
{
    public class Genre
    {
        [Key]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}