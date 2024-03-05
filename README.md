# NetcodeLearn_HumanEatCoin


## 简介: 
### 人类吃金币世界联机小游戏


## 待办: 
1. 将输入控制改为我的MyInputSystemManager
1. 加速键

## 版本迭代:


### 日志: 
#### 24.3.5
1. 发现穿模问题: 原因是代码将人物rotation.x z置0但rigidbody没有锁定xz轴旋转导致物理系统出错.
    - 解决: 将rigidbody旋转xz锁定, 这会解决穿模, 但是并不能完全锁定xz轴旋转, 还需要代码手动置0才可达到玩家不翻转效果.
1. 加个游戏内版本号区分版本, 准备开全天候Server来维持网络服, 有可能有服务器断线的问题以后再说
### 更新: 
#### 24.3.5.1
1. 增加玩家物体数据锁定变量: 
    - PlayerNComponent: 
~~~
    public bool GravityLock = true;
    public bool VelLock = true;
    public bool AVelLock = true;
    public bool RotLock = true;
    public bool RotHalfLock = true;
    public float HalfLockAngle = 45;
    public float AngleBackVelRate = 20;
~~~
#### 24.3.5.2
1. 增加游戏内版本号




### V2024.01.21-2
 - [x] 格式模板

   1. 格式模板
   2. 格式模板

 - 格式模板
 - - 格式模板
