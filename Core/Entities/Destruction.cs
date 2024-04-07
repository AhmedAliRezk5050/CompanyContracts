namespace Core.Entities;

public class Destruction : BaseEntity
{
    public decimal Amount { get; set; }

    public DateTime Date { get; set; }
    
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
}