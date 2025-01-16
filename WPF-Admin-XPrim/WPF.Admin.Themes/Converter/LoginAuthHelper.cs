using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter;

public class LoginAuthHelper {
    public static ViewAuthSwitch ViewAuthSwitch { get; set; }
    public static LoginUser? LoginUser { get; set; }

    public static void SetViewAuthSwitch(string? viewAuthSwitch) {
       var res= Enum.TryParse<ViewAuthSwitch>(viewAuthSwitch,out var auth);
       if (res)
       {
           ViewAuthSwitch = auth;
       }
    }
}