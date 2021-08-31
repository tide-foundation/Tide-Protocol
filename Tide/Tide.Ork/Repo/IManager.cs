using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public interface IManager<T> where T : IGuid, new()
    {
        Task<List<T>> GetAll();
        Task<bool> Exist(Guid id);
        Task<T> GetById(Guid id);
        Task<TideResponse> Add(T entity);
        Task<TideResponse> SetOrUpdate(T entity);
        Task Delete(Guid id);
    }
}