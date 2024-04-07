using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class DestructionRepository : BaseRepository<Destruction>, IDestructionRepository
{
    public DestructionRepository(AppDbContext context) : base(context)
    {
    }
}