namespace WPF.Admin.Models.EFDbContext.Temp;

public interface IDataRepository : IRepository<Data> {
}

public class DataRepository : RepositoryBase<Data>, IDataRepository {
    public DataRepository(SysDbContext dbContext)
        : base(dbContext) {
        
    }

    public static IDataRepository Instance { get; set; }
}