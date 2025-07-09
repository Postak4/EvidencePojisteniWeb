using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.Models.ViewModels
{
    public class TypPojisteniCardViewModel
    {
        [Display(Name = "Typ pojištění")]
        public TypPojisteni Typ { get; set; }

        [Display(Name = "Název pojištění")]
        public string Nazev { get; set; } = null!;

        [Display(Name = "Popis pojištění")]
        public string Popis { get; set; } = null!;

        [Display(Name = "Obrázek")]
        public string ObrazekUrl { get; set; } = null!;

        [Display(Name = "Dlouhý popis")]
        public string DlouhyPopis { get; set; } = null!;
    }
}
