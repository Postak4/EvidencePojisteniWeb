using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EvidencePojisteniWeb.Models
{
    /// <summary>
    /// Touto třídou dostaneme do Identity možnost „propojit“ uživatele s jeho doménovou entitou PojistenecModel.
    /// </summary>
    public class AplicationUser : IdentityUser
    {
        // FK na PojistenecModel - každý účet může mít přiřazený záznam pojištěnce
        public int ? PojistenecModelId { get; set; }

        // Navigační vlastnost pro PojistenecModel
        [ForeignKey(nameof(PojistenecModelId))]
        public virtual PojistenecModel? Pojistenec { get; set; }
    }
}
