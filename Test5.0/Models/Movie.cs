using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JCTools.GenericCrud.DataAnnotations;

namespace Test5._0.Models
{
    public class Movie
    {
        [Key]
        [Crud(Visible = false)]
        public int Id
        {
            get;
            set;
        }

        [Required(ErrorMessage="RequiredField")]
        [Display(Name = "Title", Order = 1)]
        public string Title
        {
            get;
            set;
        }
        [Display(Order = 2)]        
        public string Director
        {
            get;
            set;
        }

        [Display(Order = 4)]
        public int Year
        {
            get;
            set;
        }
        [Display(Order = 3)]  
        [Crud(UseCustomView = true)]      
        public int Country
        {
            get;
            set;
        }
    }
}