using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepositoy<T> where T :  BaseEntity
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int size)
        {
            return await _dbSet.OrderBy(x => x.Id).Skip((pageNumber - 1) * size).Take(size).ToListAsync();
        }
        //صفخه بندی و تعداد کل و مرتب سازی

        public async Task<(IEnumerable<T>, int totalCount)> GetPagedTotalAsync(IQueryable<T> query, int page, int pageSize, string? sortBy, bool descending)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var property = typeof(T).GetProperty(
                    sortBy,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);

                if (property == null)
                    throw new ArgumentException($"Invalid sort field: {sortBy}");

                query = descending
                    ? query.OrderByDescending(x => property.GetValue(x))
                    : query.OrderBy(x => property.GetValue(x));
            }
            else
            {
                query = query.OrderBy(x => x.Id);
            }
            var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalCount = await _dbSet.CountAsync();
            return (items ,  totalCount);   
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

    }
}
