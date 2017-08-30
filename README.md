DotCanal
==========

DotCanal是参考[阿里巴巴Canal](https://github.com/alibaba/canal)项目，采用[.Net Core](https://dot.net/)技术进行改写。

为什么需要改写这个项目，因为当前基于.Net Core的方案中还没有能够解决数据库层面的异地双活的较好的解决方案，大多数依赖数据库自身实现的机制在异地的情况下
会表现的十分脆弱，而Java系列在这方面拥有了很多值得参考的成功项目，得益于他们的开源，从而我们可以通过较少的成本投入将其改写为.Net Core以符合我们的整体
技术。

# 任务进度
* DotCanal.Driver：
正在编写Packet
* DotCanal.Common:
暂无依赖，只有一个日志


# 源码解析
在后期每完成一个模块后将会对这个模块从源码上进行分析。

### DotCanel.Driver（提供MySql驱动）