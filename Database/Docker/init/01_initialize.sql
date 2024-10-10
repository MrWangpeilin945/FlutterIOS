-- DB作成
CREATE DATABASE sampledb;

-- 作成したDBへ切り替え
\c sampledb

-- スキーマ作成
CREATE SCHEMA sampleschema;

-- ロールの作成
CREATE ROLE sample WITH LOGIN PASSWORD 'p@ssw0rd';

-- 権限追加
GRANT ALL PRIVILEGES ON SCHEMA sampleschema TO sample;
