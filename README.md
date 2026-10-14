# SummitWatch 中文汉化与黑名单附属（SummitWatchCN）

为 **[SummitWatch](https://thunderstore.io/c/peak/p/JavaMods/SummitWatch/)** 制作的中文附属模组，提供完整汉化与跨玩家共享的网络黑名单。

## 🌐 管理平台

**网址：[https://linling000i.github.io/SummitWatchCN/admin.html](https://linling000i.github.io/SummitWatchCN/admin.html)**

一个免费、开源的网页管理台，给所有玩家使用：

- **玩家侧**：用 Steam 登录即可查看自己的投稿/申诉状态，对被封禁的玩家提交举报、对自己的误封提交申诉、补充材料后再次提交。
- **数据全局同步**：所有管理员在网页上的操作会实时下发到所有玩家客户端，无需更新模组版本。
- **安全**：Steam OpenID 官方登录回跳校验，URL 不暴露 SteamID。

## 功能

1. **完整汉化**：Summit Watch 监视面板、迷你悬浮窗、玩家资料窗口、按钮、踢出确认弹窗、右上角提示（Toast）、检测器名称、违规理由等界面文本全部翻译为中文（含 0.1.3 新增的异常推搡检测、防甩飞救援、反复进出检测等提示）。举报文本和证据日志保持英文，不影响向作者反馈。
2. **本地黑名单**：按 **玩家名称 + 平台ID（PEAK 中即 SteamID64）** 记录黑名单玩家，进房自动识别并处理：
   - **你是主机**：立即踢出（游戏踢出 RPC + Photon 断开连接）；对方用作弊菜单无视踢出时自动重试，重试耗尽后本地屏蔽。
   - **你是队员**：可选「本地屏蔽（静音 + 在你屏幕上隐藏其角色）」「自己离开大厅」「请求主机踢出」「仅提示记录」。
3. **网络黑名单**：自动从官方服务器同步一份全站共享黑名单（以 SteamID 为主、名称为辅），与本地名单取并集生效。启动时和每隔几分钟自动同步，也可在 F8 窗口手动「立即同步」；网络不可达时自动回退到上一次缓存。

## 截图展示

Summit Watch 完整面板（Ctrl+0 打开）已完整汉化：

![SummitWatch 完整面板汉化](https://raw.githubusercontent.com/linling000i/SummitWatchCN/main/full-panel.png)

黑名单管理窗口（默认 F8 打开），已汉化并集成网络黑名单同步：

![黑名单管理窗口](https://raw.githubusercontent.com/linling000i/SummitWatchCN/main/blacklist-window.png)

## 安装

- 自动安装：在 Thunderstore / 模组管理器中安装本模组即可，会自动带上前置 **BepInEx**。
- 手动安装：先装好 BepInExPack_PEAK，再把压缩包里的 `BepInEx/plugins/SummitWatchCN/SummitWatchCN.dll` 放进游戏目录同名位置。
- SummitWatch 为可选依赖：不装 SummitWatch 时，黑名单的自动踢出/本地屏蔽/网络同步依旧独立生效，只是没有 Summit Watch 面板汉化。

## 使用方法

| 按键 | 功能 |
| --- | --- |
| `Ctrl+0` | Summit Watch 原版开关（需安装 SummitWatch）：循环切换「迷你悬浮窗 → 完整监视面板 → 关闭」 |
| `F8` | 打开/关闭本附属的**黑名单管理窗口**（默认 F8，可在配置中改键） |

黑名单窗口中可以：

- 查看当前大厅所有玩家的名称、SteamID64、主机身份；
- 一键「加入黑名单 / 移出黑名单 / 踢出（主机）/ 本地屏蔽 / 查看资料」；
- 手动填写名称或 SteamID（必须 17 位）加入黑名单；
- 查看网络黑名单同步状态、手动「立即同步」；
- 切换自动处理开关与非主机处理方式；
- 一键打开黑名单数据文件、热重载名单。

## 数据文件

```
BepInEx/config/summitwatch-blacklist.json        # 本地黑名单
BepInEx/config/summitwatch-netcache.json         # 网络黑名单缓存（自动维护）
```

- **优先按平台ID（SteamID64）匹配**，改名也逃不掉；
- 可以直接编辑本地 JSON，保存后在窗口点「重载名单」生效；
- 每加一条/删一条都会立即写入磁盘，重启游戏自动保留。

## 配置项（BepInEx/config/summitwatch.cn.blacklist.cfg）

| 配置 | 默认值 | 说明 |
| --- | --- | --- |
| `General / LocalizationEnabled` | true | 是否启用界面汉化 |
| `General / BlacklistPanelKey` | F8 | 黑名单窗口开关按键 |
| `Blacklist / Enabled` | true | 启用黑名单自动处理 |
| `Blacklist / ToastsEnabled` | true | 处理黑名单玩家时弹中文提示 |
| `Blacklist / HostAutoKick` | true | 主机自动踢出黑名单玩家 |
| `Blacklist / MatchByName` | true | 无ID条目按玩家名称匹配 |
| `Blacklist / ClientAction` | LocalBlock | 非主机时的处理：LocalBlock / LeaveRoom / AskHost / LogOnly |
| `Blacklist / KickRetrySeconds` | 3 | 踢出被无视时的重试间隔（秒） |
| `Blacklist / KickMaxRetries` | 5 | 踢出最大重试次数 |
| `Blacklist / FallbackLocalBlock` | true | 重试耗尽后本地屏蔽 |
| `Blacklist / LocalMute` | true | 本地屏蔽时静音对方 |
| `Blacklist / LocalHide` | true | 本地屏蔽时在你屏幕上隐藏对方角色 |
| `Network / EnableNetwork` | true | 是否启用网络黑名单 |
| `Network / RefreshMinutes` | 5 | 自动同步间隔（分钟） |

## 工作原理（给在意安全性的玩家）

- 汉化通过 Harmony 在 IMGUI 文本绘制接口做**运行时文本替换**，不修改任何 DLL 文件。
- 主机踢出复用游戏自带的 `PlayerHandler.Kick` 与 Photon `CloseConnection`，与 Summit Watch 自身踢出路径一致。
- 「本地屏蔽 / 静音」只在你本机生效，不发送任何网络同步；「离开大厅」是你自己断开当前房间连接。
- 网络黑名单只做**只读下载**，不会向服务器上传任何玩家信息；举报/申诉通过网页管理台提交，与模组无关。

## 前置

- BepInExPack_PEAK `5.4.75301`
- SummitWatch `0.1.3`（可选）

## 致谢

- 感谢测试人员：**咸鱼**
