using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class ContractRepository : BaseRepository<Contract>, IContractRepository
{
    public ContractRepository(AppDbContext context) : base(context)
    {
    }
}