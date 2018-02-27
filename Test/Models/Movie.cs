using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JCTools.GenericCrud.Attibutes;

namespace Test.Models
{
    public class Movie
    {
        [CrudList(Visible = false)]
        public int Id
        {
            get;
            set;
        }

        [Display(Name = "Title", Order = 1)]
        public string Title
        {
            get;
            set;
        }
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
        public string Country
        {
            get;
            set;
        }
    }
}