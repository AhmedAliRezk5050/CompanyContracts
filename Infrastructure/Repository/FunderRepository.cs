using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repository;

public class FunderRepository : BaseRepository<Funder>, IFunderRepository
{
    public FunderRepository(AppDbContext context) : base(context)
    {
    }
}