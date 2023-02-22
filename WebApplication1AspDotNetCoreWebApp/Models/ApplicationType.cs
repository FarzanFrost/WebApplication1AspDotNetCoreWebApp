using System.ComponentModel.DataAnnotations;

namespace WebApplication1AspDotNetCoreWebApp.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
