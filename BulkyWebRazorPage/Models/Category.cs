using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazorPage.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "Name field is required here.")]
        [DisplayName("Name")]
        public string? Name { get; set; }

        [DisplayName("Display Order")]
        [Range(0, 50, ErrorMessage = "Order should be in range of 0 to 50 only.")]
        public int DisplayOrder { get; set; }
    }
}
