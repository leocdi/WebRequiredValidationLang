using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebRequiredValidationLang.Models
{
    public class IndexViewModel
    {
        [Required]
        [DisplayName("La Chaine")]
        public required string Chaine { get; set; }
    }
}
