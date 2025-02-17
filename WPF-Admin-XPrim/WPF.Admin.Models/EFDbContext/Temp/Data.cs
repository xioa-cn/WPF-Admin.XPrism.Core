using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WPF.Admin.Models.EFDbContext.Temp;

[Table("Data")]
public class Data : SysEntity {
    // 主键标识
    [Key]
    // 显示名称，用于UI显示
    [Display(Name = "ID")]
    // 显示格式，10,0 表示整数格式
    [DisplayFormat(DataFormatString = "10,0")]
    // 数据库列类型为int
    [Column(TypeName = "int")]
    // 允许在UI中编辑
    [Editable(true)]
    // 必填字段，不允许空字符串
    [Required(AllowEmptyStrings = false)]
    public int Id { get; set; }

    // 显示名称为"名称"
    [Display(Name = "名称")]
    // 最大长度50个字符
    [MaxLength(50)]
    // 数据库列类型为nvarchar(50)，支持Unicode
    [Column(TypeName = "nvarchar(50)")]
    // 允许在UI中编辑
    [Editable(true)]
    // 必填字段，不允许空字符串
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }
}