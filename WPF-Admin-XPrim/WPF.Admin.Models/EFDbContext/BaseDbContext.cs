using System.Reflection;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;

namespace WPF.Admin.Models.EFDbContext;

public abstract class BaseDbContext : DbContext {
    protected abstract string ConnectionString { get; }

    public bool QueryTracking {
        set
        {
            this.ChangeTracker.QueryTrackingBehavior =
                value ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
        }
    }


    public BaseDbContext() : base() {
    }

    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) {
    }

    protected void UseDbType(DbContextOptionsBuilder optionsBuilder, string connectionString) {
        if (DBType.Instance.Name == DbCurrentType.MySql.ToString())
        {
            // 指定 MySQL 版本
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            optionsBuilder.UseMySql(connectionString, serverVersion, options =>
            {
                // 配置MySQL重试策略
                options.EnableRetryOnFailure(
                    maxRetryCount: 3, // 最多重试3次
                    maxRetryDelay: TimeSpan.FromSeconds(10), // 重试延迟10秒
                    errorNumbersToAdd: null); // 需要重试的错误码
            });
        }
        else if (DBType.Instance.Name == DbCurrentType.PgSql.ToString())
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        else if(DBType.Instance.Name == DbCurrentType.MsSql.ToString())
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected void OnModelCreating(ModelBuilder modelBuilder, Type type) {
        try
        {
            //获取所有类库
            var compilationLibrary = DependencyContext
                .Default
                .CompileLibraries
                .Where(x => !x.Serviceable && x.Type != "package" && x.Type == "project");
            foreach (var _compilation in compilationLibrary)
            {
                //加载指定类
                AssemblyLoadContext.Default
                    .LoadFromAssemblyName(new AssemblyName(_compilation.Name))
                    .GetTypes().Where(x => x.GetTypeInfo().BaseType != null
                                           && x.BaseType == (type)).ToList()
                    .ForEach(t => { modelBuilder.Entity(t); });
            }

            base.OnModelCreating(modelBuilder);
        }
        catch (Exception ex)
        {
            // TODO 文件记录
        }
    }
}