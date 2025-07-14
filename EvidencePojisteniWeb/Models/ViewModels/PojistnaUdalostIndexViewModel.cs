using System;
using System.Collections.Generic;

namespace EvidencePojisteniWeb.Models.ViewModels
{
    public class InsuranceTypeStats
    {
        public string TypPojisteni { get; set; } = string.Empty;
        public int PocetUdalosti { get; set; }
        public decimal CelkovaCastka { get; set; }
    }

    public class ClientCounts
    {
        public int CelkovyPocetKlientu { get; set; }
        public int PocetPojistniku { get; set; }
        public int PocetPojistencu { get; set; }
    }

    public class PojistnaUdalostIndexViewModel
    {
        public IEnumerable<InsuranceTypeStats> StatsByInsuranceType { get; set; } = new List<InsuranceTypeStats>();
        public ClientCounts Counts { get; set; } = new ClientCounts();
        public IEnumerable<InsuranceTypeStats> SumByInsuranceType { get; set; } = new List<InsuranceTypeStats>();
        public IEnumerable<PojistnaUdalostModel> Events { get; set; } = new List<PojistnaUdalostModel>();
        public IEnumerable<PojistenecModel> Pojistenci { get; set; } = new List<PojistenecModel>();
    }
}