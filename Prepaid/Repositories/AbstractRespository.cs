using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    /// <summary>
    /// 仓库抽象类。对模型数据的增删改查作了一个默认的实现。
    /// </summary>
    /// <typeparam name="K">数据模型的主键Key。</typeparam>
    /// <typeparam name="T">数据模型对象。</typeparam>
    public abstract class AbstractRespository<K, T> : IRepository<K, T>, IDisposable where T : class
    {
        protected PrepaidContext db = new PrepaidContext();

        public abstract DbSet<T> GetAll();

        public virtual IEnumerable<T> GetPagerItems<TKey>(int pageIndex, int pageSize, Func<T, TKey> orderFunc, bool isDesc)
        {
            int recordStart = (pageIndex - 1) * pageSize;
            if (!isDesc)
                return GetAll().OrderBy(orderFunc).Skip(recordStart).Take(pageSize);
            else
                return GetAll().OrderByDescending(orderFunc).Skip(recordStart).Take(pageSize);
        }

        public async virtual Task<T> GetByIdAsync(K uuid)
        {
            return await GetAll().FindAsync(uuid);
        }

        public T GetByID(K uuid)
        {
            return GetAll().Find(uuid);
        }

        public async virtual Task AddAsync(T item)
        {
            GetAll().Add(item);
            await db.SaveChangesAsync();
        }

        public async virtual Task PutAsync(T item)
        {
            db.Entry(item).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async virtual Task DeleteAsync(T item)
        {
            GetAll().Remove(item);
            await db.SaveChangesAsync();
        }

        public abstract bool IsExist(K uuid);

        public int GetCount()
        {
            return GetAll().Count();
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}