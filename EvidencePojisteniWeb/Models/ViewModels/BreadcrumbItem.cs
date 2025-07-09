namespace EvidencePojisteniWeb.Models.ViewModels
{
    /// <summary>
    /// Třída pro zpracování jednoduchého Breadcrumbu
    /// pro celý projekt
    /// </summary>
    public class BreadcrumbItem
    {
        public string Text { get; set; } = null!;
        public string Controller { get; set; } = null!;
        public string Action { get; set; } = null!;
        // podpora pro možnou budoucí Areas
        public string? Area { get; set; }
        // podpora pro další hodnoty trasy, např. pro URL parametry
        public IDictionary<string, string>? RouteValues { get; set; }
        public bool IsActive { get; set; }
    }
}
