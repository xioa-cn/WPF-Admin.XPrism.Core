namespace WPF.Admin.Models.Models;

public static class RegionName {
    public static string MainRegion { get; set; }
    public static string HomeRegion { get; set; }
    

    static RegionName() {
        MainRegion = nameof(MainRegion);
        HomeRegion = nameof(HomeRegion);
        
    }
}