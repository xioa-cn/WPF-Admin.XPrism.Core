using System.Data.Common;

namespace WPF.Admin.Models.EFDbContext;

public class DbConfigParameter {
    public string Name { get; set; }

    public string ConnectionString { get; set; }
}

public static class DBType {

    public static DbConfigParameter MySql { get; set; } = new DbConfigParameter() {
        Name = "MySql",
        ConnectionString = "Data Source=127.0.0.1;Database=DbName;User ID=root;Password=123456;pooling=true;CharSet=utf8;port=3306;sslmode=none;"
    };

    public static DbConfigParameter MsSql { get; set; } = new DbConfigParameter() {
        Name = "MsSql",
        ConnectionString = "Data Source=.;Initial Catalog=DbName;Persist Security Info=True;User ID=sa;Password=514224717;Connect Timeout=500;"
    };

    public static DbConfigParameter PgSql { get; set; } = new DbConfigParameter() {
        Name = "PgSql",
        ConnectionString = "Host=127.0.0.1;Port=5432;User id=postgres;password=123456;Database=DbName;"
    };
    
    public static DbConfigParameter Instance { get; set; }

    static DBType() {
        Instance = MySql;
    }
}