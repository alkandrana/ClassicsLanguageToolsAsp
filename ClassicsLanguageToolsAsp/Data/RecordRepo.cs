using System.Linq.Expressions;
using System.Reflection;
using ClassicsLanguageToolsAsp.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace WritingSessionsAspApi.Data.Contracts;

public class RecordRepo<TEntity> : IRecordRepo<TEntity> where TEntity : class
{
    private readonly AppDbContext _ctx;
    private readonly DbSet<TEntity> _dbSet;

    public RecordRepo(AppDbContext ctx)
    {
        _ctx = ctx;
        _dbSet = _ctx.Set<TEntity>();
    }
    
    public async Task<List<TEntity>> GetAllRecordsAsync()
    {
        List<TEntity> records = await _dbSet.ToListAsync();
        return records;
    }

    public async Task<List<TEntity>> GetSelectRecordsAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (include != null)
        {
            query = include(query);
        }
        List<TEntity> records = await query.ToListAsync();
        return records;
    }

    public async Task<TEntity?> GetRecordByIdAsync(int id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _dbSet;
        if (include != null)
        {
            query = include(query);
        }
        TEntity? record = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        return record;
    }

    public async Task<List<TEntity>> GetRecordByCodeAsync(string code, string propName, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        List<TEntity> records = await query.Where(e => EF.Property<string>(e, propName).ToLower() == code.ToLower()).ToListAsync();
        return records;
    }
    
    // public async Task<bool> Check

    public async Task<int> CreateRecordAsync(TEntity record)
    {
        _dbSet.Add(record);
        return await _ctx.SaveChangesAsync();
    }

    public async Task<int?> PutRecordAsync(TEntity recordData, int id)
    {
        TEntity? recordToUpdate = await _dbSet.FindAsync(id);
        if (recordToUpdate == null)
        {
            return null;
        }
        PropertyInfo[] properties = recordToUpdate.GetType().GetProperties();
        foreach (PropertyInfo prop in properties)
        {
            if (!prop.Name.ToLower().Contains("id"))
            {
                prop.SetValue(recordToUpdate, prop.GetValue(recordData));
            }
        }
        _dbSet.Update(recordToUpdate);
        return await _ctx.SaveChangesAsync();
    }

    public async Task<int> UpdateRecordAsync(TEntity record)
    {
        _dbSet.Update(record);
        return await _ctx.SaveChangesAsync();
    }
    public async Task<int> DeleteRecordAsync(TEntity record)
    {
        _dbSet.Remove(record);
        return await _ctx.SaveChangesAsync();
    }
}