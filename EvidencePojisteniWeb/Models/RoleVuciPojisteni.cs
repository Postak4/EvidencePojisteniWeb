using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.Models
{
    public enum RoleVuciPojisteni
    {
        [Display(Name = "Pojistník")]
        Pojistnik,
        [Display(Name = "Pojištěný")]
        Pojisteny
    }
}
