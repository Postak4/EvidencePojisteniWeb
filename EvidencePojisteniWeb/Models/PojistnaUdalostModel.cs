namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Model pro pojištění události
    /// </summary>
    public class PojistnaUdalostModel
    {
        public int Id { get; set; }
        public string PopisUdalosti { get; set; } = "";
        public DateTime DatumUdalosti { get; set; }
        public string Skoda { get; set; } = "";
        public bool Vyreseno { get; set; } = false;

        public int PojisteniId { get; set; } // Cizí klíč pro PojisteniModel
        public PojisteniModel? Pojisteni { get; set; } // Navigační vlastnost pro PojisteniModel
    }
}
