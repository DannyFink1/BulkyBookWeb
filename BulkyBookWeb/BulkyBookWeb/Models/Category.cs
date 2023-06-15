using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBookWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [DisplayName("Display Order")]
        public int DisplayOrdner { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
