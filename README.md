# WELLSHIP

## 外部ドキュメントのリンク

設計書、API仕様書、ER図などのリンクを掲載する。

- [UI設計書（シーケンス図）](http://10.191.26.49/wellship-docs/ui-design/)
- [アーキテクチャ設計書](http://10.191.26.49/wellship-docs/architecture/)
- [API設計書](http://10.191.26.49/wellship-docs/webapi-design/)
- [機器連携アプリ設計書](http://10.191.26.49/wellship-docs/mobileapp-design/)
- [機器連携設計書](http://10.191.26.49/wellship-docs/equipment-design/)
- [WebAPI仕様書（Redoc自動生成）](http://10.191.26.49/wellship/redoc/)
- [テーブル定義書](http://10.191.26.49/wellship-docs/database/definition/)
- [ER図](http://10.191.26.49/wellship-docs/database/erd/wellship-erd-both.pdf)

## 使用技術

### フロントエンド

- Node.js v22.9.0
- npm v
- React v19.1.0
- Mantine v7.17.4
- React Router 7.5.0

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
│   ├── アーキテクチャ設計書
│   └── 外部IF設計書
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
|`RDS_ENDPOINT_HOST`|データベースのホスト|Yes|`localhost`|
|`RDS_ENDPOINT_PORT`|データベースのポート|Yes|`15433`|

ローカル開発端末のシステム環境変数に設定するにはPowerShellでコマンドを実行する。

```powershell
[Environment]::SetEnvironmentVariable("RDS_USER_ID", "postgres", [EnvironmentVariableTarget]::Machine)
[Environment]::SetEnvironmentVariable("RDS_USER_PASS", "p@ssw0rd", [EnvironmentVariableTarget]::Machine)
[Environment]::SetEnvironmentVariable("RDS_ENDPOINT_HOST", "localhost", [EnvironmentVariableTarget]::Machine)
[Environment]::SetEnvironmentVariable("RDS_ENDPOINT_PORT", "15433", [EnvironmentVariableTarget]::Machine)
```

この例はDockerコンテナでローカルに立てたPostgreSQLを想定している。

## WebAPIテスト

### 使用ツール

[runn](https://github.com/k1LoW/runn)

YAMLでシナリオを記述して、WebAPIテストを実行するCLIツール。Go言語で書かれている。

作者はk1LoWさんであり、最近様々な企業で利用されている。

### コマンド

環境変数ファイルを指定して、複数のシナリオを実行する。

```bash
runn run .\scenarios\*.yml　--env-file .\config\.stg.env
```

### YAMLシナリオの書き方

代表的な例を記載する。

```yaml
desc: ホームメニューを取得する
runners:
  api: ${BACKEND_URL}
steps:
  getHomeMenus:
    desc: "ホームメニュー取得"
    api:
      /api/v1/homeMenus:
        get:
          body:
            application/json: null
    test: |
      current.res.status == 200
    dump:
      expr: current.res.body
      out: ../dump/homeMenus.json

```

### 補足：活用事例

- [【STAC2022】runnによるAPIのシナリオテストの導入と自動化 / 小山 健一郎さん #stac2022 - YouTube](https://www.youtube.com/watch?v=WroeAscXLdA)
- [APIシナリオテストツールとしてのrunn / 4 API testing tools - Speaker Deck](https://speakerdeck.com/k1low/4-api-testing-tools)
- [CI/CDがあたりまえの今の時代にAPIテスティングツールに求められていること / CI/CD Test Night #7 - Speaker Deck](https://speakerdeck.com/k1low/cd-test-night-number-7)
- [Web Application のテストを runn で書いて、開発と価値提供を加速する - カミナシ エンジニアブログ](https://kaminashi-developer.hatenablog.jp/entry/2024/11/26/080000)
- [runnによるAPIシナリオテスト自動化を試してみた - estie inside blog](https://www.estie.jp/blog/entry/2024/06/17/161156)
- [個人開発してるWebサービスの API のシナリオテストに runn を使ってみたけど、とてもよかった - えいのうにっき](https://blog.a-know.me/entry/2024/01/24/142328)
- [yamlでテストシナリオを書いてそのまま実行までできるAPIテストツールの新星 “runn” を試してみた | DevelopersIO](https://dev.classmethod.jp/articles/trying-runn/)

## 箱庭

ローカル端末でフロントエンド、バックエンド、データベースを起動して動作を試すことができる。

### 実行手順

1. WSL2でDockerの実行環境を作る
2. WSL2にログインしてWELLSHIPディレクトリのルートに移動する
3. `docker compose up`を実行する
4. アクセスする（デフォルトだと以下の設定）
   - フロントエンド：`http://localhost:8000`
   - バックエンド：`http://localhost:5000`
   - DB：localhost:15434;postgres;p@ssw0rd

起動中のコンテナ群を停止してビルドして再度起動するときは以下のコマンドを実行する。

```bash
docker compose down && docker compose build && docker compose up
```

### 【補足】ポートやパスワードを変更したい

WELLSHIPディレクトリのルートにある`compose.yml`を読んで変更する。

### 【補足】WSL2の有効化とDockerインストール

- [WSL のインストール | Microsoft Learn](https://learn.microsoft.com/ja-jp/windows/wsl/install)
- [WSL2+Ubuntu+Dockerでの環境整備](https://zenn.dev/pion24/articles/wsl2-ubuntu-docker_install#ubuntu%E4%B8%8A%E3%81%A7%E3%81%AEdocker%E6%95%B4%E5%82%99)
