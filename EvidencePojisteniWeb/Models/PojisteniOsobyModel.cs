namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro spojení mezi pojištěncem a pojištěním
    /// </summary>
    public class PojisteniOsobyModel
    {
        public int Id { get; set; }

        public int OsobaId { get; set; } // Cizí klíč pro PojistenecModel
        public PojistenecModel? Osoba { get; set; } // Navigační vlastnost pro PojistenecModel

        public int PojisteniId { get; set; } // Cizí klíč pro PojisteniModel
        public PojisteniModel? Pojisteni { get; set; } // Navigační vlastnost pro PojisteniModel

        public RoleVuciPojisteni Role { get; set; } // Role vůči pojištění (pojistník nebo pojištěný) 
    }
}
