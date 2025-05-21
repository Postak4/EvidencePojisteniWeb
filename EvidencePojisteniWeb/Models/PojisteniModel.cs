using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using EvidencePojisteniWeb.ValidationAttributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro pojištění
    /// </summary>
    public class PojisteniModel
    {
        // Primární klíč pro PojisteniModel
        [Key]
        public int Id { get; set; }

        // Navigační vlastnost pro PojistenecModel
        public PojistenecModel? Pojistenec { get; set; }

        // Nová kolekce – všechny záznamy propojení pojistění a osob
        public ICollection<PojisteniOsobyModel> PojisteniOsoby { get; set; } = new List<PojisteniOsobyModel>();

        [Required(ErrorMessage = "Vyberte typ pojištění")]
        [Display(Name = "Typ pojištění")]
        public TypPojisteni? TypPojisteni { get; set; }

        [Required(ErrorMessage = "Zadejte předmět pojištění")]
        [StringLength(50, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [Display(Name = "Předmět pojištění")]
        public string PredmetPojisteni { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte částku pojištění")]
        [Range(0.01, 1000000000, ErrorMessage = "{0} musí být kladné číslo mezi {1} a {2}")]
        [Display(Name = "Částka pojištění")]
        public decimal Castka { get; set; }
    }
}
