# SelfPage ![项目类型](https://img.shields.io/badge/项目类型-类库-brightgreen.svg?style=plastic)  ![目标框架](https://img.shields.io/badge/目标框架-.NetCore%202.2-brightgreen.svg?style=plastic)  ![开发工具](https://img.shields.io/badge/开发工具-Visual%20Studio%202017-brightgreen.svg?style=plastic) ![开发语言](https://img.shields.io/badge/开发语言-C%23-brightgreen.svg?style=plastic)

[项目地址](https://github.com/520xchly/SelfPage.git)：`https://github.com/520xchly/SelfPage.git`  
[src/SelfPage_Service](./src/SelfPage_Service) `服务端接口文档描述项目`  
[src/SelfPage_TestWebAPI](./src/SelfPage_TestWebAPI) `服务端测试WepAPI项目`  

## 帮助
阅读 [帮助文档](./README.md) 方便您快速了解。  
    
## NuGet包
接口文档服务[src/SelfPage_Service](./src/SelfPage_Service) Nuget包所在地址：[SelfPage.Nuget](https://www.nuget.org/packages/SelfPage_Service/)`https://www.nuget.org/packages/SelfPage_Service/`
  
    
## 快速开始   如何使用接口文档描述服务  
1、在你的 Asp.Net Core 2.2 WebApi项目中添加nuget包`SelfPage_Service`引用，用VS在线搜索nuget包即可。  
2、在 WebApi项目的StartUp.cs文件中使用[示例代码](,/src/SelfPage_TestWebAPI/Startup.cs)：  
```
app.UseSelfPage(
                //SelfPage配置信息
                opt =>
                {
                    //opt.EndPointPath = "/selfpage";  //配置接口文档路径。如：https://localhost/selfpage
                    opt.AddAuthorizationHeader = true; //接口文档中是否使用身份验证请求头
                    opt.IfUseXmlInfo = true;           //接口文档中是否使用注释信息展示（controller|action）
                    opt.Groups.AddRange(new List<string> { "manage", "v1", "v2" }); //接口分组，可不分组
                }
                ,
                //自定义分组策略，默认无分组策略。返回结果为true代表对应的controller在对应的groupname。
                (groupName, controllerInfo) =>
                {
                    return controllerInfo.ControllerRoute.StartsWith($"{groupName}");
                }
            );
```
3、运行 WebApi项目，在浏览器中输入： `https://localhost/selfpage` 即可看到接口文档。  
