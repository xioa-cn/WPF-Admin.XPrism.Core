namespace WPF.Admin.Models.EFDbContext;

public interface IRepository<TEntity> where TEntity : BaseEntity {
    BaseDbContext DbContext { get; }
}