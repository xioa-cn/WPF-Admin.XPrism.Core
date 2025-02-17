using Microsoft.EntityFrameworkCore;

namespace WPF.Admin.Models.EFDbContext;

public abstract class RepositoryBase<TEntity> where TEntity : BaseEntity {
    public RepositoryBase()
    {
    }
    public RepositoryBase(BaseDbContext dbContext)
    {
        this.DefaultDbContext = dbContext ?? throw new Exception("dbContext未实例化。");
    }
    
    private BaseDbContext DefaultDbContext { get; set; }
    private BaseDbContext EFContext
    {
        get
        {
            //DBServerProvider.GetDbContextConnection<TEntity>(DefaultDbContext);
            return DefaultDbContext;
        }
    }

    public virtual BaseDbContext DbContext
    {
        get { return DefaultDbContext; }
    }
    public DbSet<TEntity> DbSet
    {
        get { return EFContext.Set<TEntity>(); }
    }
}