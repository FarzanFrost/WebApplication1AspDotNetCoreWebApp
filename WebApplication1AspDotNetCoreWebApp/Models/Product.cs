using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1AspDotNetCoreWebApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1,int.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }

        [Display(Name="Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")] //This tells the framework to that the CategoryId is foriegn key attribute.
        public virtual Category Category { get; set; } //This automatically adds mapping between Product and Category.
                                                       //It will also create a Category Id column while will be hidden,
                                                       //to make it visible, add category id variable above in this code.

    }
}
