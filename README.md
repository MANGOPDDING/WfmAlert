# WfmAlert

[日本語](#日本語) | [English](#english)

---

## 日本語

Warframe Marketの自分の売り注文を監視・管理する、非公式のWindowsデスクトップツールです。

### 主な機能

- **In Game** の他の出品者と価格を比較します。
- 公開／非公開状態と、同じアイテムの出品数量合計を表示します。
- 一覧をダブルクリックして出品価格を変更できます。
- 行の右クリックから個別の公開状態を切り替えられます。また、すべての売り注文を一括で公開／非公開にできます。
- 自動チェックまたは即時チェックを実行し、結果やアラートをアプリ内ログに記録します。PCへの通知は行いません。
- Warframe Marketのアイテムページやトップページを開けます。

### ビルド方法

Windowsと.NET Framework 4.xが必要です。`build.bat`、`WfmAlert_market_enhancements.cs`、`WfmAlert.ico`を同じフォルダに置いて`build.bat`を実行してください。`ReleaseAsset/WfmAlert.exe`が生成されます。

### ログインとプライバシー

ソースコードにアカウントのメールアドレス、パスワード、APIトークン、Windowsユーザーのパス、アカウントslugは埋め込まれていません。アプリは必要なときに各ユーザー自身のWarframe Marketログイン情報を求めます。「ログイン情報を保存」を選ぶと、Windowsのデータ保護機能でそのWindowsユーザー向けに暗号化し、ユーザーのAppData設定ファイルに保存します。認証情報やトークンはこのリポジトリには含まれません。

このツールは非公式で、Digital ExtremesおよびWarframe Marketとは関係ありません。ログインにはWarframe Marketの旧v1方式を使用しています。この方式は非推奨かつ未サポートのため、サービスの変更により動作しなくなる可能性があります。利用・配布前に[Warframe MarketのAPIルール](https://docs.warframe.market/docs/rules/overview/)を確認してください。APIや自動化に関する方針は変更される場合があります。

### ライセンス

現在ライセンスは付属していません。GitHubで公開しても、コードの再利用・改変・再配布が自動的に許可されるわけではありません。利用を許可する場合はライセンスを追加してください。

---

## English

An unofficial Windows desktop tool for monitoring and managing your Warframe Market sell orders.

### Features

- Compare prices against other sellers who are **In Game**.
- Show public/private visibility and the total quantity listed for the same item.
- Change an order's price by double-clicking its row.
- Toggle visibility for one order from the row's right-click menu, or make all sell orders public/private.
- Run periodic checks or a one-time check, and view results and alerts in the in-app log. The app does not send PC notifications.
- Open a Warframe Market item page or the home page from the app.

### Build

Windows with .NET Framework 4.x is required. Put `build.bat`, `WfmAlert_market_enhancements.cs`, and `WfmAlert.ico` together and run `build.bat`. It creates `ReleaseAsset/WfmAlert.exe`.

### Sign-in and privacy

The source code contains no hard-coded account email, password, API token, Windows user path, or account slug. The app asks each user for their own Warframe Market credentials when needed. If “Save login” is selected, the app protects credentials with Windows Data Protection for that Windows user and stores the protected value in the user's AppData configuration file. Credentials and tokens are not included in this repository.

This tool is unofficial and is not affiliated with Digital Extremes or Warframe Market. Its sign-in currently uses Warframe Market's legacy v1 flow, which is deprecated and unsupported; it may stop working if the service changes. Review [Warframe Market's API rules](https://docs.warframe.market/docs/rules/overview/) before using or distributing the tool. API and automation policies may change.

### License

No license is included yet. Public visibility on GitHub does not by itself grant permission to reuse, modify, or redistribute this code. Add a license if you want to grant those rights.
