﻿namespace WPF.Admin.Models.Models;

public enum LoginAuth
{
    None,
    Admin,//超级权限
    FUser,//前台用户
    HUser,//后台用户
}