VideoTaskManage
======

> Desktop Application for Video synopsis, search, etc .NET Framework + WPF

## Overview

### 1. 主要功能
- 案件管理  
案件列表、案件创建、删除 ...  
- 摄像头管理  
摄像头列表、摄像头创建、删除 ...  
- 视频管理  
视频导入、列表、播放 ...  
- 视频处理  
任务列表、创建、删除 ...  
  - 视频摘要任务  
  - 视频浓缩任务  
  - 视频搜索任务  
- 标注管理  
标注列表、删除、导出、轨迹查询 ...
 
### 2. 技术内容
整体开发模式为MVVM, WPF框架 
#### 2.1 UI Implementation  
- 案件列表  
``EventView`` / ``EventViewModel``  
- 案件详情  
基础：``CameraViewModel``  
  - 摄像头列表  
~~``CameraViewListView.xaml``~~ / ~~``CameraViewListModel``~~
``CameraViewDetailListView.xaml`` / ``CameraViewDetailListModel``  
  - 摄像头地图查看
``CameraViewMapView.xaml`` / ``CameraViewMapModel``
  - 标注列表  
``PanelViewListView.xaml`` / ``PanelViewListModel``  
  - 轨迹查询  
``PanelViewPathView.xaml`` / ``PanelViewPathModel``

##### 2.1.1 查看视频  
基础：``MovieView.xaml`` / ``MovieViewModel``

- 视频列表  
``MovieViewListView.xaml`` / ``MovieViewListModel``  
- 视频播放  
``MovieViewPlayView.xaml`` / ``MovieViewPlayModel``

##### 2.1.2 视频处理  
- 添加视频  
``MovieViewModel`` -> ``ImportMovie()``  
- 视频任务  
基础：``MovieTaskViewModel``  
  - 视频处理主页面
``MovieTaskViewMainView.xaml`` / ``MovieTaskViewMainModel``
  - 视频任务列表
``MovieTaskViewListView.xaml`` / ``MovieTaskViewListModel``  
  - 视频摘要  
``MovieSummaryWindow.xaml``    
  - 视频检索  
``MovieSearchWindow.xaml``  
  - 视频浓缩
``MovieCompressWindow.xaml``   
- 视频处理结果  
基础：``PanelViewModel``  
  - 视频搜索结果  
``PanelViewTaskSearchView.xaml`` / ``PanelViewTaskSearchModel``  
  - 视频浓缩结果  
``PanelViewTaskCompressView.xaml`` / ``PanelViewTaskCompressModel``  
  - 视频摘要结果
``PanelViewTaskSummaryView.xaml`` / ``PanelViewTaskSummaryModel``

#### 2.2 功能实现
##### 2.2.1 数据管理
###### 2.2.1.1 基础：``DataItemBase``: ``ObservableCollection<DataItemBase>``  
- 有关数据库  
  - ``ItemsTable`` (``TableManager``)  
  - ``EventTable``（案件表）  
  - ``CameraTable``（摄像头表）
  - ``MovieTable``（视频表）
  - ``MovieTaskTable``（视频任务表）
###### 2.2.1.2 整体数据（``VideoData``）  
- ``AppVideoData``  
- ``ItemsTable`` -> ``EventTable``  
###### 2.2.1.3 案件对象: ``EventItem``  
- ``ItemsTable`` -> ``CameraTable``  
###### 2.2.1.4 摄像头对象: ``CameraItem``  
- ``ItemsTable`` -> ``MovieTable``  
###### 2.2.1.5 视频对象: ``MovieItem``  
``ItemsTable`` -> ``MovieTaskTable``  
- 属性; QueryVideo返回结果获取  
  - SubmitTime
  - OrgPath  
  - MovieLength
  - ~~CvtPath~~
  - State  
###### 2.2.1.6 任务对象
- 基础：``MovieTaskItem``  
- 浓缩任务: ``MovieTaskCompressItem``
  - ``CompressedPath``, ``CompressedPlayPath``: 浓缩视频名称&路径  
- 检索任务: ``MovieTaskSearchItem``  
- 摘要任务: ``MovieTaskSummaryItem``   
###### 2.2.1.7 标注对象
- ArticleItem

##### 2.2.2 API调用
###### 2.2.2.1 APIManager
- ``sendToServiceByGet()``, ``sendToServiceByPost()``  
  - ~~同步调用API~~  
  - ``asyn await``逻辑实现异步调用API
  - 解析结果xml数据 -> 返回``XElement``  
###### 2.2.2.2 API接口  
- SubmitVideo  
[http://localhost:8080/VideoAnalysisService/VideoService/SubmitVideo?OrgPath=file://camera2/mine.dav&Transcode=1]()  
State: 未开始(0) -> 任务进入待调度队列(4) -> 正在进行(1) -> 正常结束(2)  
- SubmitTask  
[http://localhost:8080/VideoAnalysisService/VideoService/SubmitTask]()  
State: 未开始(0) -> 正在进行(1) -> merge完成(4) -> 正常结束(2)

##### 2.2.3 JSON数据处理  
- ``JavaScriptSerializer(System.Web.Extensions)`` : ``String`` -> JSON对象  
```csharp
var objJSON = new JavaScriptSerializer().Deserialize<Dictionary<String, Type>>(strJSON);
```
##### 2.2.4 WebBrowser控件的利用  
- WebBrowser控件与Window.AllowsTransparency冲突问题  
AllowsTransparency为True时WebBrower不显示  
加另一个Overlay窗口放在Transparent窗口上面  
[https://blogs.msdn.microsoft.com/changov/2009/01/19/webbrowser-control-on-transparent-wpf-window/](https://blogs.msdn.microsoft.com/changov/2009/01/19/webbrowser-control-on-transparent-wpf-window/)  

- WebBrowser控件样式不兼容问题
  - 添加meta标签
``<meta http-equiv="X-UA-Compatible" content="IE=edge" /> ``
- IE上自动Allow blocked content setting  
  - ~~``<!-- saved from url=(0014)about:internet -->``~~  
  - 将html文件生成为Resource

#### 2.3 代码技巧  
- 使用MergedDictionaries分开颜色、大小等常用值  
```xml  
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="/Resources\Values.xaml" />
    <ResourceDictionary Source="/Resources\Colors.xaml" />
    ...
</ResourceDictionary.MergedDictionaries>
```  

#### 2.4 第三方库  
- vlcPlayer  
主播放器是采用vlcPlayer  
vlcPlayer.dll等相关文件
- [百度地图 JS API](http://lbsyun.baidu.com/index.php?title=jspopular3.0) v3.0  
- EventTrackBarX.ocx  

#### 2.5 相关产品&技术
- 视频浓缩摘要系统  
深圳久凌软件技术有限公司

## Need to Improve
- 优化界面 & 完善功能