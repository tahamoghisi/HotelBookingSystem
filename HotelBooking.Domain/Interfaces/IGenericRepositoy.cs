using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IGenericRepositoy<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int size);
        Task<int> GetCountAsync();
        Task<(IEnumerable<T>, int totalCount)> GetPagedTotalAsync(IQueryable<T> query, int page, int pageSize, string? sortBy, bool descending);//صفخه بندی و تعداد کل و مرتب سازی
    }
}
