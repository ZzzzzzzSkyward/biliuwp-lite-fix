# BiliLite Fix

由逍遥橙子开发的PC端B站软件我非常喜欢，但是可惜二维码登录出了点问题，所以尝试修复一下。

## 更新说明

使用VS2019，Win10 x64 1809 17763版本升级了所有依赖，但是由于很多包都要求18362版本所以我没法升到最新的。

签名换了一个，到2025年过期。

包名换了一个，版本号改了。

## 已知问题

1. 无法预约直播
2. 不能跳转到合集里的其他视频
3. 无法使用密码、短信登录，只能扫二维码
4. ...

## 下载

https://github.com/ZzzzzzzSkyward/biliuwp-lite-fix/releases

## 其他fork

最低18362版本，Mica效果https://github.com/ywmoyue/biliuwp-lite

最低18362版本，亚克力效果 https://github.com/ZzzzzzzSkyward/bilibili-uwp-liter







# 原介绍

第三方哔哩哔哩UWP客户端

下载及常见问题见：[https://www.showdoc.com.cn/biliuwpv4](https://www.showdoc.com.cn/biliuwpv4)

## 截图

![图](./screenshot/ui.png)

## 说明

运行项目需要添加FFmpeg引用。

- Nuget添加FFmpegInteropX.FFmpegUWP包。此包不支持HTTPS，可能无法正常观看直播。

- [下载](https://xiaoyaocz.lanzoui.com/i6aLtpn0kcf)并引用已编译的包，此包支持HTTPS。

	修改BiliLite.csproj里的FFmpeg路径
		
	```
	<Content Include="FFmpeg路径\$(PlatformTarget)\bin\*.dll" />
	```
	
- 自定义编译。详见[FFmpegInteropX](https://github.com/ffmpeginteropx/FFmpegInteropX)

## 参考及引用

[SYEngine](https://github.com/ShanYe/SYEngine)

[FFmpegInteropX](https://github.com/ffmpeginteropx/FFmpegInteropX)

[bilibili-API-collect](https://github.com/SocialSisterYi/bilibili-API-collect)

[bilibili-grpc-api](https://github.com/SeeFlowerX/bilibili-grpc-api)

[FontAwesome5](https://github.com/MartinTopfstedt/FontAwesome5)

[waslibs](https://github.com/wasteam/waslibs)
