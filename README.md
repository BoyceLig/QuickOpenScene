# 快速打开场景 (QuickOpenScene)

[![](https://img.shields.io/badge/Releases-下载-blue)](https://github.com/BoyceLig/QuickOpenScene/releases)
[![](https://img.shields.io/badge/203418914-QQ群-blue)](https://jq.qq.com/?_wv=1027&k=7ap29Woh)

![快速打开场景 (QuickOpenScene) 反馈群二维码](https://user-images.githubusercontent.com/49801599/213752240-a86999c1-b63d-42fc-aa81-d909297ed442.png)

快速打开场景 (QuickOpenScene) 反馈QQ群：203418914

Unity 快速打开场景工具

## 简介

此工具为方便Unity工程内有很多场景并且在不同位置，更方便的场景集合切换工具。

### 安装方法：
#### 方法1：

Package Manage添加git地址：https://github.com/BoyceLig/QuickOpenScene.git?path=Assets/QuickOpenScene

#### 方法2：

直接安装unitypackage

#### 方法3：

下载整个工程，复制Assets文件夹内的QuickOpenScene到工程里

### 自定义添加场景:

- 选定目录右键添加（`Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到配置文件`）
- 选定场景场景右键添加(`Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到配置文件`)
- 拖拽添加
- 右键全局搜索添加(`Assets/Tools/Quick Open Scene/添加所有场景到配置文件`)

以方便打开，已添加场景会自动查重，防止集合内场景重复。重复依据是路径查重。

主界面打开快捷键是 Ctrl+Alt+X ，如果有冲突请联系我更换全局快捷键。

### 主界面的功能:

左键点击对应场景打开场景

右键新增拓展功能（删除当前场景配置（不会删除场景源文件）、跳转到场景的位置、复制场景名字、复制场景路径）

- 场景需要保存则弹框提醒询问是否保存
- 场景丢失，点击打开会询问是否删除配置（不删除的情况下，同路径或者同guid点击打开对应场景事会自动刷新数据以关联新数据）
- 右侧垃圾桶图标是删除场景配置功能
- 配置文件处右键点击 `Properties...`可以呼出配置文件面板，在 `Scene Info` 栏可以调整场景的显示顺序，以及复制或者修改一些场景的数据（尽量不要在这里直接修改，三个数据是相互关联的，优先级是 `Scene>guid>path` ）。

### 配置文件位置

`UserSettings/QuickOpenSceneConfigData.json`

后期需要什么功能可以在issues里提，会慢慢往里面加功能。

## 视频功能展示

[Unity 快速打开场景工具 (QuickOpenScene)_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV1X84y1b7nU)

[Unity 快速打开场景工具 (QuickOpenScene) - YouTube](https://youtu.be/56LnPIqwjl0)

## 预览图

![image](https://user-images.githubusercontent.com/49801599/213752831-d17b0afd-27a5-4c6e-8c4d-645dc5a19d81.png)

![image](https://user-images.githubusercontent.com/49801599/212937389-640512db-5779-4660-b09d-ec27ce2bf3fd.png)

![image](https://user-images.githubusercontent.com/49801599/212937454-86266e60-cfd8-4095-977c-4a05a86bf26f.png)

![image](https://user-images.githubusercontent.com/49801599/212704250-e71ae19f-544c-42c4-9d72-211ee4e84d25.png)

![image](https://user-images.githubusercontent.com/49801599/213752999-b267977c-8ba1-479b-ab57-3fca398dc322.png)

![image](https://user-images.githubusercontent.com/49801599/213753038-b4dc43c0-92ad-4d23-a68c-135e65ab5346.png)

![image](https://user-images.githubusercontent.com/49801599/212937203-50d65da7-cdef-47a3-a37f-6259ab3c73aa.png)

![image](https://user-images.githubusercontent.com/49801599/212841692-b4720eb7-c957-4bcd-ad2a-8862ab94211c.png)

![image](https://user-images.githubusercontent.com/49801599/212841743-b0028e53-a7c2-4d38-850d-832945fffaa5.png)

![image](https://user-images.githubusercontent.com/49801599/212923658-e1d89f73-96fc-4c20-824c-0cab7591d8b5.png)

![image](https://user-images.githubusercontent.com/49801599/213754677-efeb2332-f7c9-4bb5-a860-519aaddc1c3a.png)
