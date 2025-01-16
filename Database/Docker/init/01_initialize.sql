-- DB作成
CREATE DATABASE wellship;

-- 作成したDBへ切り替え
\c wellship

-- スキーマ作成
CREATE SCHEMA resultcollector;

-- ロールの作成
CREATE ROLE wellshiprole WITH LOGIN PASSWORD 'p@ssw0rd';

-- 権限追加
GRANT ALL PRIVILEGES ON SCHEMA resultcollector TO wellshiprole;
