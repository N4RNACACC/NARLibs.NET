# AGNETS.md
## Project Overview
这是个人使用的.NET通用库，用于编写在其他项目中可能重复使用的代码，减少工作量
- 项目.NET版本: 11.0
- 项目模板类型: .NET Class library

## Development&Build
- 除非特殊声明或手动升级，此项目默认使用的.NET SDK版本为11.0，若发现.csproj内的TargetFramework与此项不符，以TargetFramework声明的版本为准
- 开始任务前先使用git pull拉取远程仓库同步改动
- 若可行，优先使用dotnet build等相关指令进行构建测试，若不可用再考虑使用msbuild
- 若可行，优先使用NuGet管理项目引用的外部软件包

## Code Specification
- summary注释块应优先使用中文
- 非必要避免使用unsafe
- 尽量使类库支持nativeAOT
- Markdown doc应使用中文编写

## Project Map
docs/* - 文档集合，统一存放不同模块的doc<br>
bin/* - 构建产物，由msbuild生成<br>
obj/* - 编译中间产物，由msbuild生成<br>
... - 各种模块<br>
.gitignore - Git文件忽略列表<br>
AGENTS.md - 此文件<br>
LICENSE.md - 开源协议<br>
README.md - 项目基础说明<br>
THIRD_PARTY_NOTICES.md - 第三方引用声明

## Agreement
- 对应模块完成后的使用方式请写入docs目录内的 模块名-doc.md 文件，而不是根目录的README.md
- 这只是一个类库，只需要按要求实现基本的可调用功能即可，保持类库的简洁轻量
- 除非特殊要求，否则不要修改根目录内的README.md文件
- 不要自行提交项目改动，与提交，PR等有关的VCS操作应由人工完成
