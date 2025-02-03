using System.ComponentModel.DataAnnotations;

namespace WebRequiredValidationLang.Models
{
    public class IndexViewModel
    {
        [Required]
        public required string Chaine { get; set; }
    }
}
