# NetcodeLearn_HumanEatCoin


## 简介: 
### 人类吃金币世界联机小游戏
java服务器端项目日志github地址: https://github.com/GoldSprite/TestNetty

本机服配置:   
- localIp: 192.168.1.105
- networkIp: 112.195.244.151
- port: 
- - 正式服:  34001
- - 测试服:  34002
---
腾讯云服配置:   
- localIp: 10.0.0.2
- networkIp: 162.14.68.248
- port: 
- - 正式服:  34001
- - 测试服:  34002



## 待办: 
1. 将输入控制改为我的MyInputSystemManager
1. 加速键
1. 增加返回键与Esc键显示玩家列表窗口
1. 制作小八模型
1. bug: 玩家退出后玩家角色不立即销毁(心跳15s), 再次加入游戏后原始角色还在造成假玩家
1. 金币不同步: 给coin做一个物体数据同步器脚本

### 已完成待办
 - [x] 加一个显示房间人数的panel(因为现在只有一个房间并没有房间列表来显示信息了)
 - [x] 在游戏内InfoTip展示玩家连接信息与识别id信息
 - [x] 明确IsConnected的获取方式, 以及明确识别指定玩家的方式(应该可以用clientId).
 - [x] 




## 版本迭代:
### 日志: 
#### 24.3.5
1. 发现穿模问题: 原因是代码将人物rotation.x z置0但rigidbody没有锁定xz轴旋转导致物理系统出错.
    - 解决: 将rigidbody旋转xz锁定, 这会解决穿模, 但是并不能完全锁定xz轴旋转, 还需要代码手动置0才可达到玩家不翻转效果.
1. 加个游戏内版本号区分版本, 准备开全天候Server来维持网络服, 有可能有服务器断线的问题以后再说
1. ping一直是-1:   
    - 已解决:   
1. 显示玩家数:   
    - 思路: 服务端管理器从NetworkManager获取客户端列表存储到变量, 由localPlayer广播到客户端

#### 24.3.6
1. 持久化开服: 正式服用34001, 开发服用34002  
    - 增加服务器ipport信息显示好知道是连的那个服  
1. 测试腾讯云: 2FPS, 2000ms, 玩不了一点  
    - 可能是性能的问题, 尝试把所有物体都删除只留玩家再试试


### 更新: 
#### 24.3.5.1
1. 增加玩家物体数据锁定变量: 
    - PlayerNComponent: 
```
    public bool GravityLock = true;
    public bool VelLock = true;
    public bool AVelLock = true;
    public bool RotLock = true;
    public bool RotHalfLock = true;
    public float HalfLockAngle = 45;
    public float AngleBackVelRate = 20;
```

#### 24.3.5.2
1. 增加游戏内版本号

#### 24.3.5.3
1. 修复创建与加入房间逻辑
1. 增加连接状态, 本地NId, 房间玩家数显示

#### 24.3.6.0
1. infoTip太小了安卓看不清, 调整布局大小与边距
1. 增加serverip-port显示
1. PC窗口改为resizeWindow

#### 24.3.7.0
1. 创建C#NettyClient测试, 尝试连接javaNettyServer本地服
    - 完成本地互联
1. 总结签入: 
    - 导入NuGetForUnity包以及其DotNetty相关包
    - 增加TestNettyClient基本代码
    - [重点!]: PlayerSettings Player otherSettings 里面关闭assemblyVersionValidation才能运行外部包(也就是NuGet相关包), 不记得报错也会告诉你

#### 24.3.13.0
1. netty不行dotnetty bug太多, 换socket了
2. 完成本地通信

#### 24.3.13.1-尝试云服定点通信
1. 完成对向通信

---  
---  
### V2024.01.21-2
 - [x] 格式模板

   1. 格式模板
   2. 格式模板

 - 格式模板
 - - 格式模板


---
## 学习手册

想要获取Client列表可以通过  (仅服务端可访问)
NetworkManager: IReadOnlyList<NetworkClient> ConnectedClientsList

获取当前系统毫秒时间戳
DateTime.Now.TimeOfDay.TotalMilliseconds

NetworkBehaviour 几大类型区别  
 - IsServer: 服务端且本机持有权物体
 - IsLocalPlayer: 本地玩家
 - IsOwner: 本机持有权物体
 - IsOwnedByServer: 服务器持有权物体

