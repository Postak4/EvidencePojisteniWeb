using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro spojení mezi pojištěncem a pojištěním
    /// </summary>
    public class PojisteniOsobyModel
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key pro Pojištěnce
        /// </summary>
        [Required]
        [Display(Name = "Pojištěnec")]
        public int OsobaId { get; set; }

        // Navigační vlastnost pro PojistenecModel
        [ForeignKey(nameof(OsobaId))]
        public PojistenecModel? Osoba { get; set; }

        /// <summary>
        /// Foreign key pro Pojištění
        /// </summary>
        [Required]
        [Display(Name = "Pojištění")]
        public int PojisteniId { get; set; }

        // Navigační vlastnost pro PojisteniModel
        [ForeignKey(nameof(PojisteniId))]
        public PojisteniModel? Pojisteni { get; set; }

        // Role vůči pojištění (pojistník nebo pojištěný)
        [Required(ErrorMessage = "Vyberte roli")]
        [Display(Name = "Role vůči pojištění")]
        public RoleVuciPojisteni? Role { get; set; } 
    }
}
