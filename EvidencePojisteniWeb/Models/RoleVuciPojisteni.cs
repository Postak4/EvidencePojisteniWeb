using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.Models
{
    public enum RoleVuciPojisteni
    {
        [Display(Name = "Pojistník")]
        Pojistnik = 0,
        [Display(Name = "Pojištěná osoba")]
        Pojisteny = 1
    }
}
