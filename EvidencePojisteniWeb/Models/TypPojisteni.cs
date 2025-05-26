using System.ComponentModel.DataAnnotations;

namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Typ pojištění enum pro výběr typu pojištění
    /// </summary>
    public enum TypPojisteni
    {
        [Display(Name = "Životní pojištění")]
        Zivotni = 1,
        [Display(Name = "Úrazové pojištění")]
        Urazove = 2,
        [Display(Name = "Cestovní pojištění")]
        Cestovni = 3,
        [Display(Name = "Pojištění domácnosti")]
        Majetkove = 4,
        [Display(Name = "Pojištění odpovědnosti za škodu")]
        Odpovednost = 5
    }
}
