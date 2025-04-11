using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using OutbornE_commerce.DAL.Data;
using OutbornE_commerce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.BaseRepositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> FindAllAsync(string[] includes, bool withNoTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (withNoTracking)
                query = query.AsNoTracking();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.ToListAsync();
        }
        public async Task<PagainationModel<IEnumerable<T>>> FindAllAsyncByPagination(Expression<Func<T, bool>>? criteria = null, int pageNumber = 1, int pageSize = 10, string[] includes = null)
        {
            int totalCount = 0;
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (criteria != null)
                query = query.Where(criteria);

            totalCount = query.Count();

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            var data = await query.ToListAsync();
            return new PagainationModel<IEnumerable<T>>()
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        //string[] includes = new string[] { "SubCategories" };
        public async Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task<T?> Find(Expression<Func<T, bool>> expression, bool trackChanges = false, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return !trackChanges ?
            await query.Where(expression).AsNoTracking().FirstOrDefaultAsync()
            : await query.Where(expression).AsTracking(QueryTrackingBehavior.TrackAll).FirstOrDefaultAsync();
        }
        public async Task<T?> FindIncludesSplited(Expression<Func<T, bool>> expression, bool trackChanges = false, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return !trackChanges ?
            await query.Where(expression).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync()
            : await query.Where(expression).AsTracking(QueryTrackingBehavior.TrackAll).AsSplitQuery().FirstOrDefaultAsync();
        }
        public async Task<T> Create(T entity)
        {
            var result = await _context.Set<T>().AddAsync(entity);
            return result.Entity;
        }
        public async Task CreateRange(List<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public void Delete(T entity) => _context.Set<T>().Remove(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void UpdateRange(List<T> entities) => _context.Set<T>().UpdateRange(entities);
        public async Task DeleteRange(Expression<Func<T, bool>> expression) => await _context.Set<T>().Where(expression).ExecuteDeleteAsync();

        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? expression = null)
        {
            if (expression == null)
            {
               
                return await _context.Set<T>().CountAsync();
            }
            else
            {
                return await _context.Set<T>().Where(expression).CountAsync();
            }
        }


        public async Task BeginTransactionAsync()
          => await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
          => await _context.Database.CommitTransactionAsync();

        public async Task RollbackTransactionAsync()
         => await _context.Database.RollbackTransactionAsync();
    }
}
