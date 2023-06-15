using Microsoft.EntityFrameworkCore;
using RestfulAPICRUDExample.Data;
using RestfulAPICRUDExample.Models;
using RestfulAPICRUDExample.Repository.IRepository;
using System.Linq;
using System.Linq.Expressions;

namespace RestfulAPICRUDExample.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _context;
        public VillaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Villa entity)
        {
            await _context.Villas.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Villa> GetAsync(Expression<Func<Villa,bool>> filter = null, bool tracked = true)
        {
            IQueryable<Villa> query = _context.Villas;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        

        public async Task<List<Villa>> GetAllAsync(Expression<Func<Villa,bool>> filter = null)
        {
            IQueryable<Villa> query = _context.Villas;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await _context.Villas.ToListAsync();
        }

        public async Task RemoveAsync(Villa entity)
        {
            _context.Villas.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Villa entity)
        {
            _context.Villas.Update(entity);
            await SaveAsync();
        }
    }
}
