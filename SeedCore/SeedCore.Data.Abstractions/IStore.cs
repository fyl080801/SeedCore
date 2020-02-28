using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeedCore.Data
{
    /// <summary>
    /// 数据访问层持久化接口
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbContext CreateDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDbContext CreateDbContext(IServiceProvider serviceProvider);

        /// <summary>
        /// 创建一个数据访问上下文
        /// </summary>
        /// <returns></returns>
        IDbContext CreateDbContext(IEnumerable<object> typeConfigs);

        /// <summary>
        /// 创建一个数据库配置
        /// </summary>
        /// <returns></returns>
        DbContextOptions CreateOptions(bool cached = false);

        // /// <summary>
        // /// 初始化数据访问
        // /// </summary>
        // /// <param name="service"></param>
        // /// <returns></returns>
        // Task InitializeAsync(IServiceProvider serviceProvider);
    }
}
