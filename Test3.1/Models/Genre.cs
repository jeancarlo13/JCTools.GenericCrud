using System.ComponentModel.DataAnnotations;
using JCTools.GenericCrud.DataAnnotations;

namespace Test3._1.Models
{
    public class Genre
    {
        [Key]
        [Crud(IsEditableKey = true)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}