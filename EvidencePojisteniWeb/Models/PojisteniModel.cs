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

        // Cizí klíč pro PojistenecModel
        [Required]
        [Display(Name = "Pojištěnec")]
        public int PojistenecId { get; set; }

        // Navigační vlastnost pro PojistenecModel
        public PojistenecModel? Pojistenec { get; set; }

        // Nová kolekce – všechny záznamy propojení pojistění a osob
        public ICollection<PojisteniOsobyModel> PojisteniOsoby { get; set; } = new List<PojisteniOsobyModel>();

        [Required(ErrorMessage = "Vyberte typ pojištění")]
        [Display(Name = "Typ pojištění")]
        public TypPojisteni? TypPojisteni { get; set; }

        [Required(ErrorMessage = "Zadejte předmět pojištění")]
        [StringLength(50, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [RegularExpression(@"^[\p{L}'\-\s]{2,40}$", ErrorMessage = "{0} může obsahovat pouze písmena (2–40 znaků)")]
        [Display(Name = "Předmět pojištění")]
        public string PredmetPojisteni { get; set; } = "";

        /// <summary>
        /// Datum začátku pojištění - nesmí být v minulosti
        /// </summary>
        [Required(ErrorMessage = "Zadejte datum začátku")]
        [DataType(DataType.Date)]
        [DateNotInPast(ErrorMessage = "Datum začátku pojištění nesmí být v minulosti")]
        [Display(Name = "Platnost od")]
        public DateTime DatumZacatku { get; set; }

        /// <summary>
        /// Datum konce pojištění - nesmí být za datem začátku
        /// </summary>
        [Required(ErrorMessage = "Zadejte datum konce")]
        [DataType(DataType.Date)]
        [DateGreaterThan(nameof(DatumZacatku), ErrorMessage = "Datum konce pojištění musí být po datu začátku")]
        [Display(Name = "Platnost do")]
        public DateTime DatumKonce { get; set; }

        [Required(ErrorMessage = "Vyplňte částku pojištění")]
        [Range(0.01, 1000000000, ErrorMessage = "{0} musí být kladné číslo mezi {1} a {2}")]
        [Display(Name = "Částka pojištění")]
        public decimal Castka { get; set; }

    }
}
