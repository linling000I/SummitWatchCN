# 更新日志

本文件记录 SummitWatchCN 的所有版本变更，遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/) 风格。

## [1.0.2] - 2026-10-14

### 新增
- 集成网页管理台 [https://summitwatchcn.icu](https://summitwatchcn.icu)：玩家可通过网页举报作弊玩家、对误封提交申诉；管理员在网页审核、封禁、限时赦免。
- 网络黑名单节点：优先从 `https://api.summitwatchcn.icu/blacklist` 拉取，失败时自动回退到 GitHub raw；本地缓存 `summitwatch-netcache.json`，启动时预加载。
- 网络黑名单条目增加玩家名称显示。
- SteamID 输入框增加 17 位位数校验。
- 主机踢出任何玩家时自动加入本地黑名单。

### 修复
- 修复嵌套花括号解析失败导致网络黑名单无法加载的问题。
- 修复重启游戏后网络黑名单缓存未生效的问题。
- 修复新数据条目数为 0 时错误清空本地缓存的问题。

### 变更
- 客户端版本号锁定为 1.0.2，节点顺序与缓存逻辑稳定。

## [1.0.1] - 2026-10-12

### 新增
- 网络黑名单（GitHub `blacklist.json` 自动同步，可单独开关）。
- 本地/网络双名单并集生效。
- 限时赦免观察期（可自定义天数）。

### 变更
- 解除对 SummitWatch 的硬依赖：未装 SummitWatch 时，黑名单依旧独立运行。

## [1.0.0] - 2026-10-11

### 首次发布
- SummitWatch 面板 / 迷你悬浮窗 / 资料窗口 / Toast 提示全汉化（适配 SummitWatch 0.1.3）。
- 本地黑名单：名称 + SteamID 持久化、主机自动踢出与重试、客户端本地屏蔽 / 离开大厅 / 请求主机踢出。
- F8 黑名单管理窗口。
