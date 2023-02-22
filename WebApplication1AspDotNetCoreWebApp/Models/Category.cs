using System.ComponentModel.DataAnnotations;

namespace WebApplication1AspDotNetCoreWebApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //added validations
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="display Order for category must be greater than 0")]
        [Display(Name = "Display Order")]
        public string DisplayOrder { get; set; }
    }
}
