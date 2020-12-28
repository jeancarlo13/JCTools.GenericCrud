using System.ComponentModel.DataAnnotations;

namespace Test3._1.Models
{
    public class Genre
    {
        [Key]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}