# 快速打开场景 (QuickOpenScene)

Unity 快速打开场景工具

## 简介

此工具为方便Unity工程内有很多场景并且在不同位置，更方便的场景集合切换工具。

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

`Assets/QuickOpenScene/Data/SceneConfig.asset`

**说明:配置文件为自动索引，可以随意拖动插件的目录，但是不可以配置文件的相对路径，相对路径必须在 `QuickOpenScene/Data/SceneConfig.asset`**

后期需要什么功能可以在issues里提，会慢慢往里面加功能。

## 视频功能展示

[Unity 快速打开场景工具 (QuickOpenScene)_哔哩哔哩_bilibili](https://www.bilibili.com/video/BV1X84y1b7nU)

[Unity 快速打开场景工具 (QuickOpenScene) - YouTube](https://youtu.be/56LnPIqwjl0)

## 预览图

![image](https://user-images.githubusercontent.com/49801599/212704223-d0d961e0-aa40-4246-ae1a-cd10e2a54f7c.png)

![image](https://user-images.githubusercontent.com/49801599/212937389-640512db-5779-4660-b09d-ec27ce2bf3fd.png)

![image](https://user-images.githubusercontent.com/49801599/212937454-86266e60-cfd8-4095-977c-4a05a86bf26f.png)

![image](https://user-images.githubusercontent.com/49801599/212704250-e71ae19f-544c-42c4-9d72-211ee4e84d25.png)

![image](https://user-images.githubusercontent.com/49801599/212704415-eddb3bae-c1f2-498f-aba3-aea65f7710d8.png)

![image](https://user-images.githubusercontent.com/49801599/212937203-50d65da7-cdef-47a3-a37f-6259ab3c73aa.png)

![image](https://user-images.githubusercontent.com/49801599/212841692-b4720eb7-c957-4bcd-ad2a-8862ab94211c.png)

![image](https://user-images.githubusercontent.com/49801599/212841743-b0028e53-a7c2-4d38-850d-832945fffaa5.png)

![image](https://user-images.githubusercontent.com/49801599/212923658-e1d89f73-96fc-4c20-824c-0cab7591d8b5.png)
