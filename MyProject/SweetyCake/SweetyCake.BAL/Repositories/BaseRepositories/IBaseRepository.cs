using Microsoft.EntityFrameworkCore;
using OutbornE_commerce.BAL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Repositories.BaseRepositories
{
	public interface IBaseRepository<T> where T : class
	{
        Task<IEnumerable<T>> FindAllAsync(string[] includes, bool withNoTracking = true);
        Task<PagainationModel<IEnumerable<T>>> FindAllAsyncByPagination(
           Expression<Func<T, bool>>? criteria = null,
           int pageNumber = 1,
           int pageSize = 10,
           string[] includes = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
        Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> criteria, string[] includes = null);

        Task<T?> Find(Expression<Func<T, bool>> expression, bool trackChanges = false, string[] includes = null);
        Task<T?> FindIncludesSplited(Expression<Func<T, bool>> expression, bool trackChanges = false, string[] includes = null);

        Task<T> Create(T entity);
		Task CreateRange(List<T> entities);

        void Delete(T entity);
		Task DeleteRange(Expression<Func<T, bool>> expression);
		void UpdateRange(List<T> entities);

        void Update(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>>? expression = null);

        Task SaveAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync();

        Task CommitTransactionAsync();


        Task RollbackTransactionAsync();

    }
}
