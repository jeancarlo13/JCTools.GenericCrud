using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JCTools.GenericCrud.DataAnnotations;

namespace Test5._0.Models
{
    [Display(Name = nameof(Country), ResourceType = typeof(Resources.I18NTest))]
    public class Country
    {
        [Key]
        [Crud(Visible = false)]
        public int Id
        {
            get;
            set;
        }

        [Required(ErrorMessage = "RequiredField")]
        [Display(Name = "Name", Order = 1)]
        public string Name
        {
            get;
            set;
        }
    }
}