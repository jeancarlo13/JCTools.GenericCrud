using System.ComponentModel.DataAnnotations;
using JCTools.GenericCrud.DataAnnotations;

namespace Test.Models
{
    public class Genre
    {
        [Key]
        [Crud(IsEditableKey = true)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}