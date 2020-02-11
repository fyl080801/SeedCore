using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace SeedCore.Data
{
    /// <summary>
    /// 实现读取序列化数据的 DbSet
    /// </summary>
    /// <remarks>
    /// 不支持直接使用异步方法（FirstOrDefaultAsync 之类的）
    /// </remarks>
    public class DocumentDbSet<TEntity> :
        DbSet<TEntity>,
        IQueryable<TEntity>,
        IAsyncEnumerable<TEntity>,
        IInfrastructure<IServiceProvider>
        where TEntity : class
    {
        readonly IDbContext _dbcontext;

        public DocumentDbSet(IDbContext dbcontext) : base()
        {
            _dbcontext = dbcontext;
        }

        public override LocalView<TEntity> Local => base.Local;

        public override EntityEntry<TEntity> Add(TEntity entity)
        {
            return base.Add(entity);
        }

        public override ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(entity, cancellationToken);
        }

        public override void AddRange(params TEntity[] entities)
        {
            base.AddRange(entities);
        }

        public override void AddRange(IEnumerable<TEntity> entities)
        {
            base.AddRange(entities);
        }

        public override Task AddRangeAsync(params TEntity[] entities)
        {
            return base.AddRangeAsync(entities);
        }

        public override Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return base.AddRangeAsync(entities, cancellationToken);
        }

        public override IAsyncEnumerable<TEntity> AsAsyncEnumerable()
        {
            return base.AsAsyncEnumerable();
        }

        public override IQueryable<TEntity> AsQueryable()
        {
            return base.AsQueryable();
        }

        public override EntityEntry<TEntity> Attach(TEntity entity)
        {
            return base.Attach(entity);
        }

        public override void AttachRange(params TEntity[] entities)
        {
            base.AttachRange(entities);
        }

        public override void AttachRange(IEnumerable<TEntity> entities)
        {
            base.AttachRange(entities);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override TEntity Find(params object[] keyValues)
        {
            return base.Find(keyValues);
        }

        public override ValueTask<TEntity> FindAsync(params object[] keyValues)
        {
            return base.FindAsync(keyValues);
        }

        public override ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            return base.FindAsync(keyValues, cancellationToken);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override EntityEntry<TEntity> Remove(TEntity entity)
        {
            return base.Remove(entity);
        }

        public override void RemoveRange(params TEntity[] entities)
        {
            base.RemoveRange(entities);
        }

        public override void RemoveRange(IEnumerable<TEntity> entities)
        {
            base.RemoveRange(entities);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override EntityEntry<TEntity> Update(TEntity entity)
        {
            return base.Update(entity);
        }

        public override void UpdateRange(params TEntity[] entities)
        {
            base.UpdateRange(entities);
        }

        public override void UpdateRange(IEnumerable<TEntity> entities)
        {
            base.UpdateRange(entities);
        }

        #region old
        // readonly DbSet<Document> _document;
        // readonly IEnumerable<PropertyInfo> _keyCollection;
        // readonly Type _entityType;
        // readonly Type _documentType = typeof(Document);
        // readonly IQueryable<Document> _documentQuery;

        // // IQueryable<TEntity> _entityQuery;
        // LocalView<TEntity> _local;

        // public DocumentDbSet(IDbContext dbContext)
        // {
        //     _dbContext = dbContext;
        //     _document = ((IDocumentDbContext)dbContext).Document;
        //     _entityType = typeof(TEntity);
        //     var entityTypeName = _entityType.ToString();
        //     _documentQuery = _document.Where(e => e.Type == entityTypeName);

        //     var documentKeys = typeof(Document).GetProperties()
        //         .Where(e => e.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0)
        //         .AsEnumerable();

        //     _keyCollection = _entityType.GetProperties()
        //         .Where(e => e.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0 && documentKeys.Any(x => x.Name == e.Name && x.PropertyType == e.PropertyType))
        //         .AsEnumerable();
        // }

        // public override EntityEntry<TEntity> Add(TEntity entity)
        // {
        //     var document = new Document()
        //     {
        //         Type = _entityType.ToString(),
        //         Content = JsonConvert.SerializeObject(entity)
        //     };
        //     _document.Add(document);
        //     _dbContext.SaveChanges();
        //     ResolveKeyValue(document, entity);
        //     return null;
        // }

        // public override ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        // {
        //     if (cancellationToken.IsCancellationRequested)
        //     {
        //         return new ValueTask<EntityEntry<TEntity>>(Task.FromCanceled<EntityEntry<TEntity>>(cancellationToken));
        //     }

        //     return new ValueTask<EntityEntry<TEntity>>(Add(entity));
        // }

        // public override void AddRange(params TEntity[] entities)
        // {
        //     AddRange((entities ?? new TEntity[0]).AsEnumerable());
        // }

        // public override void AddRange(IEnumerable<TEntity> entities)
        // {
        //     foreach (var entity in entities)
        //     {
        //         Add(entity);
        //     }
        // }

        // public override Task AddRangeAsync(params TEntity[] entities)
        // {
        //     return AddRangeAsync((entities ?? new TEntity[0]).AsEnumerable());
        // }

        // public override Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        // {
        //     return Task.Run(() => AddRange(entities), cancellationToken);
        // }

        // public override IAsyncEnumerable<TEntity> AsAsyncEnumerable()
        // {
        //     return base.AsAsyncEnumerable();
        // }

        // public override IQueryable<TEntity> AsQueryable()
        // {
        //     return base.AsQueryable();
        // }

        // public override EntityEntry<TEntity> Attach(TEntity entity)
        // {
        //     return base.Attach(entity);
        // }

        // public override void AttachRange(params TEntity[] entities)
        // {
        //     base.AttachRange(entities);
        // }

        // public override void AttachRange(IEnumerable<TEntity> entities)
        // {
        //     base.AttachRange(entities);
        // }

        // public override bool Equals(object obj)
        // {
        //     return base.Equals(obj);
        // }

        // public override TEntity Find(params object[] keyValues)
        // {
        //     var document = _document.Find(keyValues);
        //     return document == null ? null : ResolveKeyValue(document, JsonConvert.DeserializeObject<TEntity>(document.Content));
        // }

        // public override ValueTask<TEntity> FindAsync(params object[] keyValues)
        // {
        //     return new ValueTask<TEntity>(Find(keyValues));
        // }

        // public override ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        // {
        //     if (cancellationToken.IsCancellationRequested)
        //     {
        //         return new ValueTask<TEntity>(Task.FromCanceled<TEntity>(cancellationToken));
        //     }

        //     return new ValueTask<TEntity>(Task.FromResult(Find(keyValues)));
        // }

        // public override int GetHashCode()
        // {
        //     return base.GetHashCode();
        // }

        // public override EntityEntry<TEntity> Remove(TEntity entity)
        // {
        //     var keys = _keyCollection.Select(e => e.GetValue(entity)).ToArray();
        //     var document = _document.Find(keys);
        //     _document.Remove(document);
        //     return null;
        // }

        // public override void RemoveRange(params TEntity[] entities)
        // {
        //     RemoveRange((entities ?? new TEntity[0]).AsEnumerable());
        // }

        // public override void RemoveRange(IEnumerable<TEntity> entities)
        // {
        //     foreach (var entity in entities)
        //     {
        //         Remove(entity);
        //     }
        // }

        // public override string ToString()
        // {
        //     return base.ToString();
        // }

        // public override EntityEntry<TEntity> Update(TEntity entity)
        // {
        //     var keys = _keyCollection.Select(e => e.GetValue(entity)).ToArray();
        //     var document = _document.Find(keys);
        //     document.Content = JsonConvert.SerializeObject(entity);
        //     _document.Update(document);
        //     return null;
        // }

        // public override void UpdateRange(params TEntity[] entities)
        // {
        //     RemoveRange((entities ?? new TEntity[0]).AsEnumerable());
        // }

        // public override void UpdateRange(IEnumerable<TEntity> entities)
        // {
        //     foreach (var entity in entities)
        //     {
        //         Update(entity);
        //     }
        // }

        // private TEntity ResolveKeyValue(Document document, TEntity entity)
        // {
        //     foreach (var key in _keyCollection)
        //     {
        //         key.SetValue(entity, _documentType.GetProperty(key.Name).GetValue(document));
        //     }
        //     return entity;
        // }
        #endregion
    }
}
