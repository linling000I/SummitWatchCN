# SummitWatch 中文汉化与黑名单附属（SummitWatchCN）

为 **[SummitWatch](https://thunderstore.io/c/peak/p/JavaMods/SummitWatch/)** 制作的中文附属模组，提供两大功能：

1. **完整汉化**：Summit Watch 监视面板、迷你悬浮窗、玩家资料窗口、按钮、踢出确认弹窗、右上角提示（Toast）、检测器名称、违规理由等界面文本全部翻译为中文（含 0.1.3 新增的异常推搡检测、防甩飞救援、反复进出检测等提示）。举报文本和证据日志保持英文，不影响向作者反馈。
2. **黑名单**：按 **玩家名称 + 平台ID（PEAK 中即 SteamID64）** 记录黑名单玩家，进房自动识别并处理：
   - **你是主机**：立即踢出（游戏踢出 RPC + Photon 断开连接）；对方用作弊菜单无视踢出时自动重试，重试耗尽后可在本地屏蔽。
   - **你是队员**：可选「本地屏蔽（静音 + 在你屏幕上隐藏其角色）」「自己离开大厅（断开连接）」「请求主机踢出（主机装了 Summit Watch 时）」「仅提示记录」。

## 截图展示

Summit Watch 完整面板（Ctrl+0 打开）已完整汉化：

![SummitWatch 完整面板汉化](https://raw.githubusercontent.com/linling000I/SummitWatchCN/main/screenshots/full-panel.png)

黑名单管理窗口（默认 F8 打开）：

![黑名单管理窗口](https://raw.githubusercontent.com/linling000I/SummitWatchCN/main/screenshots/blacklist-window.png)

## 安装

- 自动安装：在 Thunderstore / 模组管理器中安装本模组即可，会自动带上前置 **BepInEx** 和 **SummitWatch**。
- 手动安装：先装好 BepInExPack_PEAK 与 SummitWatch，再把压缩包里的 `BepInEx/plugins/SummitWatchCN/SummitWatchCN.dll` 放进游戏目录同名位置。

## 使用方法

| 按键 | 功能 |
| --- | --- |
| `Ctrl+0` | Summit Watch 原版开关（0.1.3 起）：循环切换「迷你悬浮窗 → 完整监视面板 → 关闭」，均已汉化；可在 SummitWatch 配置 `General/PanelShortcut` 改键 |
| `F8` | 打开/关闭本附属的**黑名单管理窗口**（默认 F8，可在配置中改键；不用 F3 是为避开 NVIDIA 游戏滤镜快捷键） |

黑名单窗口中可以：

- 查看当前大厅所有玩家的名称、平台ID（SteamID64）、主机身份；
- 一键「加入黑名单 / 移出黑名单 / 踢出（主机）/ 本地屏蔽 / 查看资料」；
- 查看、复制ID、移除全部历史黑名单条目；
- 手动填写名称或 SteamID 加入黑名单；
- 切换自动处理开关与非主机处理方式；
- 一键打开黑名单数据文件、热重载名单。

## 数据文件

黑名单保存在 `BepInEx/config/summitwatch-blacklist.json`。

- **优先按平台ID（SteamID64）匹配**，最准确，改名也逃不掉；
- 只有当条目没有ID时，才会按名称匹配（不区分大小写，可在配置中关闭）；
- 可以直接编辑该 JSON，保存后在窗口点「重载名单」生效；
- **每加一条/删一条都会立即写入磁盘**（先写临时文件再原子替换），重启游戏或电脑后名单自动保留，无需手动保存。

## 前置与版本

- BepInExPack_PEAK `5.4.75301`
- SummitWatch `0.1.3`

## 更新日志

### 1.0.0

- 首个版本：SummitWatch 面板/迷你悬浮窗/资料/提示全汉化（适配 0.1.3）；黑名单（名称 + SteamID）持久化、主机自动踢出与重试、客户端本地屏蔽/离开大厅/请求主机踢出、F8 管理窗口。

## 致谢

- 感谢测试人员：**咸鱼**
