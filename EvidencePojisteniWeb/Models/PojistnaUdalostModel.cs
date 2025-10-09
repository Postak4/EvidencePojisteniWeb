using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EvidencePojisteniWeb.ValidationAttributes;

namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro pojistnou událost.
    /// </summary>
    public class PojistnaUdalostModel
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key pro Pojištěnce
        /// </summary>
        [Required]
        [Display(Name = "Pojištěnec")]
        public int OsobaId { get; set; }

        /// <summary>
        /// Navigační vlastnost pro PojistenecModel
        /// </summary>
        [ForeignKey(nameof(OsobaId))]
        public PojistenecModel? Osoba { get; set; }

        /// <summary>
        /// FK na pojištění, ke kterému událost patří.
        /// </summary>
        [Required]
        [Display(Name = "Pojištění")]
        public int PojisteniId { get; set; }

        /// <summary>
        /// Navigační vlastnost na pojištění.
        /// </summary>
        [ForeignKey(nameof(PojisteniId))]
        public PojisteniModel? Pojisteni { get; set; }

        /// <summary>
        /// Popis události (max 250 znaků).
        /// </summary>
        [Required(ErrorMessage = "Vyplňte popis události")]
        [StringLength(250, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [Display(Name = "Popis události")]
        public string PopisUdalosti { get; set; } = "";

        /// <summary>
        /// Datum, kdy k události došlo (nesmí být v budoucnosti).
        /// </summary>
        [Required(ErrorMessage = "Vyplňte datum události")]
        [DataType(DataType.Date)]
        [DateNotInFuture(ErrorMessage = "Datum události nemůže být v budoucnosti")]
        [Display(Name = "Datum události")]
        public DateTime DatumUdalosti { get; set; }

        /// <summary>
        /// Místo události (max 100 znaků).
        /// </summary>
        [Required(ErrorMessage = "Zadejte místo, kde k události došlo")]
        [StringLength(100, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [Display(Name = "Místo události")]
        public string MistoUdalosti { get; set; } = "";

        /// <summary>
        /// Přibližná výše škody v Kč.
        /// </summary>
        [Required(ErrorMessage = "Vyplňte přibližnou škodu události")]
        [Range(0, 100_000_000, ErrorMessage = "{0} musí být mezi {1} a {2}")]
        [Display(Name = "Škoda v Kč")]
        public decimal Skoda { get; set; }

        /// <summary>
        /// Stav vyřešení události.
        /// </summary>
        [Display(Name = "Vyřešeno")]
        public bool Vyreseno { get; set; } = false;

        /// <summary>
        /// Jméno svědka (2–40 písmen).
        /// </summary>
        [StringLength(40, MinimumLength = 2, ErrorMessage = "{0} musí mít 2–40 znaků")]
        [Display(Name = "Jméno svědka")]
        public string? Svedek { get; set; }

        /// <summary>
        /// Celá adresa svědka (max 200 znaků).
        /// </summary>
        [StringLength(200, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [Display(Name = "Adresa svědka")]
        public string? AdresaSvedka { get; set; }

        /// <summary>
        /// Dodatečná poznámka.
        /// </summary>
        [DataType(DataType.MultilineText)]
        [StringLength(1000, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [Display(Name = "Poznámka")]
        public string? Poznamka { get; set; }

    }
}
