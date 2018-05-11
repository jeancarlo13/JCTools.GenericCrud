using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Genre
    {
        [Key]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}