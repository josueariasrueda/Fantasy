using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fantasy.Shared.Entities.Domain
{
    public class AccountingAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } // Código de la cuenta contable

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Nombre de la cuenta contable

        [StringLength(200)]
        public string Description { get; set; } // Descripción opcional

        [Required]
        public AccountType Type { get; set; } // Tipo de cuenta (Activo, Pasivo, etc.)

        public bool IsActive { get; set; } = true; // Indica si la cuenta está activa

        // Campo para agrupar cuentas
        public int? ParentAccountId { get; set; } // Id de la cuenta principal (puede ser nulo)

        [ForeignKey("ParentAccountId")]
        public AccountingAccount? ParentAccount { get; set; } // Relación con la cuenta principal

        [Required]
        public int TenantId { get; set; } // Identificador del tenant
    }

    public enum AccountType
    {
        Asset,      // Activo
        Liability,  // Pasivo
        Equity,     // Patrimonio
        Revenue,    // Ingreso
        Expense     // Gasto
    }
}