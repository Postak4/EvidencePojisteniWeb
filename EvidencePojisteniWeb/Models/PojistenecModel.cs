using System.ComponentModel.DataAnnotations;
using EvidencePojisteniWeb.ValidationAttributes;


namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro pojištěnce
    /// </summary>
    public class PojistenecModel
    {
        // Primární klíč pro PojistenecModel
        public int Id { get; set; }

        // Zde je kolekce pojištění, které má pojištěnec
        public ICollection<PojisteniModel>? Pojisteni { get; set; } = new List<PojisteniModel>();

        [Required(ErrorMessage = "Vyplňte jméno")]
        [StringLength(50, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [RegularExpression(@"^[\p{L}'\-\s]{2,40}$", ErrorMessage = "{0} může obsahovat pouze písmena (2–40 znaků)")]
        [Display(Name = "Jméno")]
        public string Jmeno { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte příjmení")]
        [StringLength(50, ErrorMessage = "{0} může mít nejvýše {1} znaků")]
        [RegularExpression(@"^[\p{L}'\-\s]{2,40}$", ErrorMessage = "{0} může obsahovat pouze písmena (2–40 znaků)")]
        [Display(Name = "Příjmení")]
        public string Prijmeni { get; set; } = "";

        [Required(ErrorMessage = "Datum narození je povinné")]
        [DataType(DataType.Date)]
        [BirthDate]                                           // Vlastní atribut pro validaci data narození
        [Display(Name = "Datum narození")]
        public DateTime DatumNarozeni { get; set; }

        [Required(ErrorMessage = "Adresa je povinná")]
        [StringLength(50, ErrorMessage = "{0} musí mít alespoň {2} znaků a nejvíce {1}", MinimumLength = 4)]
        [Display(Name = "Ulice a číslo popisné/číslo evidenční")]
        public string UliceCpCe { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte město")]
        [StringLength(50, ErrorMessage = "{0} musí mít alespoň {2} znaků a nejvíce {1}", MinimumLength = 2)]
        [Display(Name = "Město")]
        public string Mesto { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte PSČ")]
        [StringLength(10, ErrorMessage = "{0} musí mít alespoň {2} znaků a nejvíce {1}", MinimumLength = 5)]
        [RegularExpression(@"^(?:\d{5}|\d{3}\s\d{2})$",
            ErrorMessage = "{0} musí mít 5 číslic nebo 3 číslice a 2 číslice oddělené mezerou (např. 12345 nebo 123 45)")]
        [Display(Name = "PSČ")]
        public string PSC { get; set; } = "";

        // Mohl by se přidat enum pro výběr státu - zatím nerealizováno
        [Required(ErrorMessage = "Vyplňte stát")]
        [StringLength(50, ErrorMessage = "{0} musí mít alespoň {2} znaků a nejvíce {1}", MinimumLength = 3)]
        [Display(Name = "Stát")]
        public string Stat { get; set; } = "Česká republika";

        [Required(ErrorMessage = "Vyplňte telefon")]
        [StringLength(20, ErrorMessage = "{0} musí mít alespoň {2} znaků a nejvíce {1}", MinimumLength = 9)]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "{0} musí mít alespoň 9 číslic a maximálně 15 číslic (např. +420123456789)")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = "";

        [Required(ErrorMessage = "Vyplňte e-mailovou adresu")]
        [EmailAddress(ErrorMessage = "Zadejte platnou e-mailovou adresu")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = "";

    }
}
