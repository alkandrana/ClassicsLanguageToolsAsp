using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
namespace WritingSessionsAspApi.Data.Contracts;

public interface IRecordRepo<TEntity>
{
    public Task<List<TEntity>> GetAllRecordsAsync();
    public Task<List<TEntity>> GetSelectRecordsAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    
    public Task<TEntity?> GetRecordByIdAsync(int id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    
    public Task<List<TEntity>> GetRecordByCodeAsync(string code, string propName, params Expression<Func<TEntity, object>>[] includes);
    
    public Task<int> CreateRecordAsync(TEntity record);
    
    public Task<int> UpdateRecordAsync(TEntity record);
    
    public Task<int?> PutRecordAsync(TEntity recordData, int id);
    
    public Task<int> DeleteRecordAsync(TEntity record);
}