using System.ComponentModel.DataAnnotations;

namespace Test5._0.Models
{
    [Display(Name = nameof(Genre), ResourceType = typeof(Resources.I18NTest))]
    public class Genre
    {
        [Key]
        [Display(Name = nameof(Name), ResourceType = typeof(Resources.I18NTest))]
        public string Name { get; set; }

        [Display(Name = nameof(Description), ResourceType = typeof(Resources.I18NTest))]
        public string Description { get; set; }
    }
}