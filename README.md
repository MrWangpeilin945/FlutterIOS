# WELLSHIP

## 外部ドキュメントのリンク

TBD  
設計書、API仕様書、ER図などのリンクを掲載する。

- [アーキテクチャ設計書](http://10.191.26.49/wellship-docs/architecture/)
- [WebAPI仕様書（設計用）](http://10.191.26.49/wellship-docs/webapi-design/)
- [WebAPI仕様書（自動生成）](http://10.191.26.49/wellship/redoc/)
- [WebAPIカバレッジレポート](http://10.191.26.49/wellship-tests/)

## システム構成

TBD  
実装のためのシステム構成図を掲載する。

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
```

### .NET

.NET SDKをインストールする。

```bash
winget install Microsoft.DotNet.SDK.8
```

## ディレクトリ構造

モノレポ構成とする。

Excelで作成するUI設計書やマニュアルはこのGitリポジトリでは管理しない。

```txt
.
├── Documents/
│   ├── テーブル定義書
│   └── WebAPI仕様書
├── Database/
│   ├── schema/
│   │   └── create-database.sql
│   └── migration  
├── Frontend/
│   └── 結果収集
└── Backend/
    ├── WELLSHIP.sln
    ├── src/
    │   ├── WELLSHIP.Core/
    │   │   ├── WELLSHIP.Core.csproj
    │   │   ├── Exceptions
    │   │   └── Logging
    │   ├── WELLSHIP.WebAPI/
    │   │   ├── WELLSHIP.WebAPI.csproj
    │   │   └── 結果収集/
    │   │       ├── Controllers
    │   │       ├── Usecases
    │   │       ├── Domain/
    │   │       │   ├── Enums
    │   │       │   ├── Models
    │   │       │   └── Repositories
    │   │       └── Infrastructure/
    │   │           └── RepositoryImpls
    │   ├── WELLSHIP.APIModels/
    │   │   ├── WELLSHIP.APIModels.csproj
    │   │   └── 結果収集/
    │   │       ├── Requests
    │   │       └── Responses
    │   └── WELLSHIP.CLI（仮置き）/
    │       └── WELLSHIP.CLI.csproj（仮置き）
    └── tests/
        ├── WELLSHIP.WebAPI.Tests/
        │   └── WELLSHIP.WebAPI.Tests.csproj/
        │       └── 結果収集/
        │           ├── Usecase
        │           └── Domain/
        │               └── Model
        ├── WELLSHIP.CLI.Tests（仮置き）/
        │   └── WELLSHIP.CLI.Tests.csproj（仮置き）
        └── IntegrationTests
```

## Gitの運用ルール

TBD  
Git-Flowを採用する予定。

## 環境変数

|名称|説明|必須|例|
|-----|-----|-----|-----|
|`RDS_USER_ID`|データベースのユーザID|Yes||
|`RDS_USER_PASS`|データベースのパスワード|Yes||
|`RDS_ENDPOINT`|データベースのエンドポイント|Yes||
