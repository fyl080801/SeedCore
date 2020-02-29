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
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Collections;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

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
        // IEnumerable<TEntity>,
        // IEnumerable,
        IAsyncEnumerable<TEntity>,
        IInfrastructure<IServiceProvider>
        where TEntity : class
    {
        readonly IDbContext _dbcontext;
        readonly DbSet<Document> _document;
        readonly IQueryable<Document> _query;
        readonly Type _entityType;
        readonly Type _documentType;
        readonly IEnumerable<PropertyInfo> _keys;

        // IQueryable<TEntity> _entityQuery;
        // LocalView<TEntity> _local;

        public DocumentDbSet(IDbContext dbcontext) : base()
        {
            _dbcontext = dbcontext;
            _document = dbcontext.Document;
            _entityType = typeof(TEntity);
            _documentType = typeof(Document);
            var entityTypeName = _entityType.ToString();
            _query = _document.Where(e => e.Type == entityTypeName);

            var documentKeys = typeof(Document).GetProperties()
                .Where(e => e.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0)
                .AsEnumerable();

            _keys = _entityType.GetProperties()
                .Where(e => e.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0 && documentKeys.Any(x => x.Name == e.Name && x.PropertyType == e.PropertyType))
                .AsEnumerable();

            if (_keys.Count() <= 0)
            {
                _keys = _entityType.GetProperties().Where(e => e.Name == "Id");
            }
        }

        // public override LocalView<TEntity> Local
        //        => _local ?? (_local = new LocalView<TEntity>(this));

        public override EntityEntry<TEntity> Add(TEntity entity)
        {
            var document = new Document()
            {
                Type = _entityType.ToString(),
                Content = JsonConvert.SerializeObject(entity)
            };
            _document.Add(document);
            _dbcontext.SaveChanges();
            // 为了将 Document 添加后的 key 对应到实体里
            ResolveKeyValue(document, entity);
            return null;
        }

        public override ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<EntityEntry<TEntity>>(Task.FromCanceled<EntityEntry<TEntity>>(cancellationToken));
            }

            return new ValueTask<EntityEntry<TEntity>>(Add(entity));
        }

        public override void AddRange(params TEntity[] entities)
        {
            var typeName = _entityType.ToString();
            var documents = entities
                .Select(e => new Document()
                {
                    Type = typeName,
                    Content = JsonConvert.SerializeObject(e)
                })
                .ToArray();
            _document.AddRange(documents);
            _dbcontext.SaveChanges();
        }

        public override void AddRange(IEnumerable<TEntity> entities)
        {
            AddRange(entities.ToArray());
        }

        public override Task AddRangeAsync(params TEntity[] entities)
        {
            return Task.Run(() => AddRange(entities));
        }

        public override Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => AddRange(entities), cancellationToken);
        }

        public override IAsyncEnumerable<TEntity> AsAsyncEnumerable()
        {
            return _query.Select(e => JsonConvert.DeserializeObject<TEntity>(e.Content)).AsAsyncEnumerable();
        }

        public override IQueryable<TEntity> AsQueryable()
        {
            return _query.Select(e => JsonConvert.DeserializeObject<TEntity>(e.Content)).AsQueryable();
        }

        public override TEntity Find(params object[] keyValues)
        {
            var document = _document.Find(keyValues);

            return document == null
                ? null
                : ResolveKeyValue(document, JsonConvert.DeserializeObject<TEntity>(document.Content));
        }

        public override ValueTask<TEntity> FindAsync(params object[] keyValues)
        {
            return new ValueTask<TEntity>(Find(keyValues));
        }

        public override ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<TEntity>(Task.FromCanceled<TEntity>(cancellationToken));
            }

            return new ValueTask<TEntity>(Task.FromResult(Find(keyValues)));
        }

        public override EntityEntry<TEntity> Remove(TEntity entity)
        {
            var keys = _keys.Select(e => e.GetValue(entity)).ToArray();
            var document = _document.Find(keys);
            _document.Remove(document);
            return null;
        }

        public override void RemoveRange(params TEntity[] entities)
        {
            RemoveRange((entities ?? new TEntity[0]).AsEnumerable());
        }

        public override void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override EntityEntry<TEntity> Update(TEntity entity)
        {
            Document document = null;
            if (_keys.Count() > 0)
            {
                var keys = _keys.Select(e => e.GetValue(entity)).ToArray();
                document = _document.Find(keys);
            }
            else
            {
                document = _query.FirstOrDefault();
            }

            if (document != null)
            {
                document.Content = JsonConvert.SerializeObject(entity);
                _document.Update(document);
            }

            return null;
        }

        public override void UpdateRange(params TEntity[] entities)
        {
            RemoveRange((entities ?? new TEntity[0]).AsEnumerable());
        }

        public override void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        private IQueryable<TEntity> GetEntityQuery()
        {
            return _query.ToArray()
                .Select(e => ResolveKeyValue(e, JsonConvert.DeserializeObject<TEntity>(e.Content)))
                .AsQueryable();
        }
        private TEntity ResolveKeyValue(Document document, TEntity entity)
        {
            foreach (var key in _keys)
            {
                key.SetValue(entity, _documentType.GetProperty(key.Name).GetValue(document));
            }
            return entity;
        }

        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        private IQueryable<TEntity> EntityQuery
            => GetEntityQuery();

        public Type ElementType
            => _entityType;

        public IQueryProvider Provider
            => EntityQuery.Provider;

        public Expression Expression
            => EntityQuery.Expression;

        public IServiceProvider Instance
            => _document.GetInfrastructure();

        public IEnumerator<TEntity> GetEnumerator()
        {
            return EntityQuery.GetEnumerator();
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return EntityQuery.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        #region not support
        public override EntityEntry<TEntity> Attach(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public override void AttachRange(params TEntity[] entities)
        {
            throw new NotSupportedException();
        }

        public override void AttachRange(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
}
