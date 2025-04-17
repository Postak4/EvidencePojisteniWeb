using Microsoft.AspNetCore.Mvc.Rendering;

namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro pojištění
    /// </summary>
    public class PojisteniModel
    {
        // Primární klíč pro PojisteniModel
        public int Id { get; set; }

        // Cizí klíč pro PojistenecModel
        public int PojistenecId { get; set; }

        // Navigační vlastnost pro PojistenecModel
        public PojistenecModel? Pojistenec { get; set; } 

        public string TypPojisteni { get; set; } = "";
        public string PredmetPojisteni { get; set; } = "";
        public DateTime DatumZacatku { get; set; }
        public DateTime DatumKonce { get; set; }
        public decimal Castka { get; set; }
        public List<SelectListItem> DruhyPojisteni { get; set; } 

        public PojisteniModel()
        {
            // Inicializace seznamu druhů pojištění
            DruhyPojisteni =
                [
                    new SelectListItem { Value = "Životní pojištění", Text = "Životní pojištění" },
                    new SelectListItem { Value = "Úrazové pojištění", Text = "Úrazové pojištění" },
                    new SelectListItem { Value = "Cestovní pojištění", Text = "Cestovní pojištění" },
                    new SelectListItem { Value = "Pojištění domácnosti", Text = "Pojištění domácnosti" },
                    new SelectListItem { Value = "Pojištění odpovědnosti", Text = "Pojištění odpovědnosti" }
                ];
        }
    }
}
