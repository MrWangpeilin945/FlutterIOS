# WELLSHIP

## 外部ドキュメントのリンク

設計書、API仕様書、ER図などのリンクを掲載する。

- [UI設計書（シーケンス図）](http://10.191.26.49/wellship-docs/ui-design/)
- [アーキテクチャ設計書](http://10.191.26.49/wellship-docs/architecture/)
- [API設計書](http://10.191.26.49/wellship-docs/webapi-design/)
- [WebAPI仕様書（自動生成）](http://10.191.26.49/wellship/redoc/)
- [WebAPIカバレッジレポート](http://10.191.26.49/wellship-tests/)

## 使用技術

### フロントエンド

- Node.js v22.9.0
- npm v
- React v18.2.0
- Mantine v7.12.0
- Remix SPA Mode 2.12.1

### バックエンド・CLIツール

- C# 12
- NET 8.0
- ASP.NET Core

### 機器連携アプリ

- Flutter v

## EditorConfig

本プロジェクトでは以下のスタイルを原則とする。
意図せずバージョン管理の差分が出るため、統一いただきたい。

|項目名|設定値|
|-----|-----|
|文字コード|UTF-8 BOMなし|
|改行コード|CRLF|

ただし、プログラミング言語やフレームワーク固有の慣習があればそちらに従う。

## 環境構築の手順

### Node.js

Node.jsのバージョン共存のために[NVM for Windows](https://github.com/coreybutler/nvm-windows)を使用する。

NVM for Windowsをインストールする。

```bash
winget install CoreyButler.NVMforWindows
```

必要なバージョンのNode.jsをインストールする

```bash
nvm install 22.4.1
```

バージョンを指定してNode.jsを切り替える

```bash
nvm use 22.4.1
```

選択中のバージョンを確認する

```bash
node -v
```

### .NET

.NET SDKをインストールする。

```bash
winget install Microsoft.DotNet.SDK.8
```

## ディレクトリ構造

モノレポ構成とする。

Excelで作成するUI設計書やマニュアルはこのGitリポジトリでは管理しない。
ただしUI設計書で使うシーケンス図はMermaid.jsで記述してAsciidocでまとめて管理する。

```txt
.
├── Documents/
│   ├── API設計書
│   ├── UI設計書
│   └── アーキテクチャ設計書
├── Database/
│   ├── Docker
│   └── ERD
├── Frontend/
│   └── ResultCollector/
│       ├── app
│       ├── build
│       ├── public
│       └── vite.config.ts
└── Backend/
    ├── WELLSHIP.sln
    ├── src/
    │   ├── Wellship.APIModels
    │   ├── Wellship.Core
    │   └── Wellship.WebAPI
    └── tests/
        ├── Wellship.Core.Tests
        └── Wellship.WebAPI.Tests
```

## Gitの運用方針

ブランチ戦略はGit-Flowを採用する。

<https://www.atlassian.com/ja/git/tutorials/comparing-workflows/gitflow-workflow>

### Git-Flowについて

![img](https://wac-cdn.atlassian.com/dam/jcr:34c86360-8dea-4be4-92f7-6597d4d5bfae/02%20Feature%20branches.svg)
フロー図はatlassian.comから引用。

|種類|用途|ブランチ名|
|------|-------|--------|
| Main | 本番環境用。バージョンごとにタグをつける | `main` |
| Release | 検証環境テスト用 | `release` |
| Develop | 開発用 | `develop` |
| Feature | 機能開発ブランチ。実装者がDevelopから切る| `feature/{WELLSHIP_DEV-XXX}/{実装概要を英単語で}` |

### 機能開発の流れ

1. 最新のdevelopブランチを取得する
2. developブランチからfeatureブランチを切る
3. 実装してcommitする
4. featureブランチをリモートにpushする
5. Backlogの画面でfeatureからdevelopに向けてプルリクエストを作成する
6. コードレビュー後、プルリクエストがマージされる

競合が発生した場合は、レビュイーが解決する。

### ブランチ名の付け方

ブランチ名はBacklogの課題番号を含める。

- `feature/{WELLSHIP_DEV-XXX}/{実装概要を英単語で}`
- `feature/WELLSHIP_DEV-XXX/sc0004-home`

### プルリクエストの出し方

Backlogの操作方法はサポートページを参照。

<https://support-ja.backlog.com/hc/ja/articles/360035640574-%E3%83%97%E3%83%AB%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E3%81%AE%E4%BD%BF%E7%94%A8%E6%96%B9%E6%B3%95>

#### 件名

Backlog課題名が自動で入力されるので、課題名の後ろに変更内容を簡潔に書く。

- `[画面_実装]SC0001_ログイン画面 初期実装`
- `[画面_実装]SC0001_ログイン画面 レイアウト変更対応`

#### 内容

```txt
## 対応内容

## 影響範囲

## レビュー観点

## 補足

```

### 設定

プルリクエストを作成する際は以下を設定する。

| 入力項目 | 設定値 | 詳細 |
| ------- | ------ | ----|
|ターゲットブランチ|`develop`|マージ先のブランチ|
|プルリクエストブランチ|`feature/{課題キー}/{概要}`|マージしたいブランチ|
|担当者|実装者|プルリクエストの担当|
|関連課題|実装課題のキー|ブランチ名に課題キーがあれば自動で紐づく|
|お知らせしたいユーザー|-|プルリクエストの追加時に通知したいユーザー|

### Gitクライアント

プロジェクトとして指定ツールはなし。お好みのツールをご利用ください。

- gitコマンド
- TortoiseGit
- Visual Studio Code

## 環境変数

### バックエンド

|名称|説明|必須|例|
|-----|-----|-----|-----|
|`RDS_USER_ID`|データベースのユーザID|Yes|`postgres`|
|`RDS_USER_PASS`|データベースのパスワード|Yes|`p@ssw0rd`|
|`RDS_ENDPOINT`|データベースのエンドポイント|Yes|`Host=localhost;Port=15433;Database=dev01;`|

ローカル開発端末のシステム環境変数に設定するにはPowerShellでコマンドを実行する。

```powershell
[Environment]::SetEnvironmentVariable("RDS_USER_ID", "postgres", [EnvironmentVariableTarget]::Machine)
[Environment]::SetEnvironmentVariable("RDS_USER_PASS", "p@ssw0rd", [EnvironmentVariableTarget]::Machine)
[Environment]::SetEnvironmentVariable("RDS_ENDPOINT", "Host=localhost;Port=15433;Database=dev01;", [EnvironmentVariableTarget]::Machine)
```

この例はDockerコンテナでローカルに立てたPostgreSQLを想定している。
