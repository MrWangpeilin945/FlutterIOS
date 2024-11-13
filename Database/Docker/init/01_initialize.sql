-- DB作成
CREATE DATABASE dev01;

-- 作成したDBへ切り替え
\c dev01

-- スキーマ作成
CREATE SCHEMA resultcollector;

-- ロールの作成
CREATE ROLE dev01role WITH LOGIN PASSWORD 'p@ssw0rd';

-- 権限追加
GRANT ALL PRIVILEGES ON SCHEMA resultcollector TO dev01role;
