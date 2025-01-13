﻿namespace WPF.Admin.Themes.Themes;

/// <summary>
/// 系统图标路径数据
/// </summary>
public class IconPaths {
    public string? this[string key] {
        get
        {
            var property = this.GetType().GetProperties();
            return (from item in property let 
                name = item.Name where name == key select item.GetValue(this) as string).FirstOrDefault();
        }
    }

    /// <summary>
    /// 登录图标 
    /// </summary>
    public string Login { get; } =
        "M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z";

    /// <summary>
    /// 数据管理图标 
    /// </summary>
    public string DataList { get; } =
        "M14 2H6c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V8l-6-6zM6 20V4h7v5h5v11H6zm2-6h8v2H8v-2zm0-4h8v2H8v-2z";

    /// <summary>
    /// 搜索图标 
    /// </summary>
    public string Search { get; } =
        "M15.5 14h-.79l-.28-.27a6.5 6.5 0 1 0-.7.7l.27.28v.79l4.25 4.25c.41.41 1.08.41 1.49 0 .41-.41.41-1.08 0-1.49L15.5 14zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z";

    /// <summary>
    /// 数据列表图标 
    /// </summary>
    public string Pagination { get; } =
        "M3 13h2v-2H3v2zm0 4h2v-2H3v2zm0-8h2V7H3v2zm4 4h14v-2H7v2zm0 4h14v-2H7v2zM7 7v2h14V7H7z";

    /// <summary>
    /// 数据可视化图标 
    /// </summary>
    public string Visualization { get; } =
        "M5 9.2h3V19H5V9.2zM10.6 5h2.8v14h-2.8V5zm5.6 8H19v6h-2.8v-6z";

    /// <summary>
    /// 图表分析图标 
    /// </summary>
    public string Charts { get; } =
        "M3.5 18.5l6-6 4 4L20.5 6M20.5 6v5h-5";

    /// <summary>
    /// Excel图标 
    /// </summary>
    public string Excel { get; } =
        "M19 3H5c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 2v3H5V5h14zM5 19v-9h14v9H5zm2-4h2v2H7v-2zm4 0h2v2h-2v-2z";

    /// <summary>
    /// 代码编辑图标 
    /// </summary>
    public string CodeEdit { get; } =
        "M9.4 16.6L4.8 12l4.6-4.6L8 6l-6 6 6 6 1.4-1.4zm5.2 0l4.6-4.6-4.6-4.6L16 6l6 6-6 6-1.4-1.4z";

    /// <summary>
    /// 错误诊断图标 
    /// </summary>
    public string Error { get; } =
        "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 11c-.55 0-1-.45-1-1V8c0-.55.45-1 1-1s1 .45 1 1v4c0 .55-.45 1-1 1zm1 4h-2v-2h2v2z";

    /// <summary>
    /// 二维码图标 
    /// </summary>
    public string QrCode { get; } =
        "M3 11h8V3H3v8zm2-6h4v4H5V5zm8-2v8h8V3h-8zm6 6h-4V5h4v4zM3 21h8v-8H3v8zm2-6h4v4H5v-4zm13-2h3v2h-3v-2zm-3 0h2v2h-2v-2zm3 6h3v2h-3v-2zm-3 0h2v2h-2v-2z";

    /// <summary>
    /// Web浏览器图标 
    /// </summary>
    public string WebView { get; } =
        "M20 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 14H4V8h16v10z";

    /// <summary>
    /// 流程图图标 
    /// </summary>
    public string FlowChart { get; } =
        "M3 3h6v4H3V3zm12 0h6v4h-6V3zm-6 5v8h6V8H9zm-6 9h6v4H3v-4zm12 0h6v4h-6v-4zm-2-4l-4-3v6l4-3z";

    /// <summary>
    /// 软件主题图标
    /// </summary>
    public string Theme { get; } =
        "M12 3c-4.97 0-9 4.03-9 9s4.03 9 9 9c.83 0 1.5-.67 1.5-1.5 0-.39-.15-.74-.39-1.01-.23-.26-.38-.61-.38-.99 0-.83.67-1.5 1.5-1.5H16c2.76 0 5-2.24 5-5 0-4.42-4.03-8-9-8zm-5.5 9c-.83 0-1.5-.67-1.5-1.5S5.67 9 6.5 9 8 9.67 8 10.5 7.33 12 6.5 12zm3-4C8.67 8 8 7.33 8 6.5S8.67 5 9.5 5s1.5.67 1.5 1.5S10.33 8 9.5 8zm5 0c-.83 0-1.5-.67-1.5-1.5S13.67 5 14.5 5s1.5.67 1.5 1.5S15.33 8 14.5 8zm3 4c-.83 0-1.5-.67-1.5-1.5S16.67 9 17.5 9s1.5.67 1.5 1.5-.67 1.5-1.5 1.5z";

    /// <summary>
    /// 配色方案图标
    /// </summary>
    public string ColorPalette { get; } =
        "M17.5 12c-2.89 0-5.25 2.35-5.25 5.25S14.61 22.5 17.5 22.5c1.29 0 2.42-.47 3.33-1.24.58-.5.92-1.21.92-1.96 0-.86-.51-1.6-1.24-1.96.73-.36 1.24-1.09 1.24-1.96 0-1.2-.96-2.17-2.17-2.17h-.08c.22-.7.33-1.43.33-2.17 0-2.76-1.69-5.13-4.08-6.13.27.81.41 1.66.41 2.54 0 1.77-.57 3.44-1.65 4.82.46.06.93.09 1.41.09.5 0 1-.04 1.48-.13.78.82 1.26 1.92 1.26 3.12 0 .09-.01.18-.01.27zM6.5 10C4.01 10 2 7.99 2 5.5S4.01 1 6.5 1 11 3.01 11 5.5 8.99 10 6.5 10z";

    /// <summary>
    /// 轮播图图标
    /// </summary>
    public string Carousel { get; } =
        "M19,3H5C3.89,3 3,3.89 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5C21,3.89 20.1,3 19,3M19,19H5V5H19V19M13.96,12.29L11.21,15.83L9.25,13.47L6.5,17H17.5L13.96,12.29Z";
    // 这是一个示例图标路径，代表一个带有图片效果的轮播图标

    /// <summary>
    /// 视觉集成图标
    /// </summary>
    public string Vision { get; } =
        "M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z";

    /// <summary>
    /// VisionPro图标
    /// </summary>
    public string VisionPro { get; } =
        "M12,9A3,3 0 0,1 15,12A3,3 0 0,1 12,15A3,3 0 0,1 9,12A3,3 0 0,1 12,9M12,4.5C17,4.5 21.27,7.61 23,12C21.27,16.39 17,19.5 12,19.5C7,19.5 2.73,16.39 1,12C2.73,7.61 7,4.5 12,4.5M3.18,12C4.83,15.36 8.24,17.5 12,17.5C15.76,17.5 19.17,15.36 20.82,12C19.17,8.64 15.76,6.5 12,6.5C8.24,6.5 4.83,8.64 3.18,12Z";

    /// <summary>
    /// 无限滚动图标
    /// </summary>
    public string InfiniteScroll { get; } =
        "M12 4V1L8 5l4 4V6c3.31 0 6 2.69 6 6 0 1.01-.25 1.97-.7 2.8l1.46 1.46C19.54 15.03 20 13.57 20 12c0-4.42-3.58-8-8-8zm0 14c-3.31 0-6-2.69-6-6 0-1.01.25-1.97.7-2.8L5.24 7.74C4.46 8.97 4 10.43 4 12c0 4.42 3.58 8 8 8v3l4-4-4-4v3z";

    /// <summary>
    /// 自定义组件图标
    /// </summary>
    public string CustomComponent { get; } =
        "M3 3h8v8H3V3m2 2v4h4V5H5m8-2h8v8h-8V3m2 2v4h4V5h-4M3 13h8v8H3v-8m2 2v4h4v-4H5m13-2h3v2h-3v3h-2v-3h-3v-2h3V8h2v3h3v2z";

    /// <summary>
    /// 刷新Token图标
    /// </summary>
    public string RefreshToken { get; } =
        "M17.65 6.35C16.2 4.9 14.21 4 12 4c-4.42 0-7.99 3.58-7.99 8s3.57 8 7.99 8c3.73 0 6.84-2.55 7.73-6h-2.08c-.82 2.33-3.04 4-5.65 4-3.31 0-6-2.69-6-6s2.69-6 6-6c1.66 0 3.14.69 4.22 1.78L13 11h7V4l-2.35 2.35z";

    /// <summary>
    /// 甘特图图标
    /// </summary>
    public string GanttChart { get; } =
        "M4 5h16v2H4zm0 6h4v2H4zm0 6h8v2H4zm6-6h10v2H10zm4 6h6v2h-6z";


    /// <summary>
    /// 关于信息图标
    /// </summary>
    public string About { get; } =
        "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z";

    /// <summary>
    /// 版本信息图标
    /// </summary>
    public string Version { get; } =
        "M20 4H4c-1.11 0-1.99.89-1.99 2L2 18c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V6c0-1.11-.89-2-2-2zm0 14H4v-6h16v6zm0-10H4V6h16v2z";

    /// <summary>
    /// 系统信息图标
    /// </summary>
    public string SystemInfo { get; } =
        "M13 7h-2v2h2V7zm0 4h-2v6h2v-6zm4-9.99L7 1c-1.1 0-2 .9-2 2v18c0 1.1.9 2 2 2h10c1.1 0 2-.9 2-2V3c0-1.1-.9-1.99-2-1.99zM17 19H7V5h10v14z";

    /// <summary>
    /// 许可证图标
    /// </summary>
    public string License { get; } =
        "M12 1L3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4zm0 10.99h7c-.53 4.12-3.28 7.79-7 8.94V12H5V6.3l7-3.11v8.8z";
}