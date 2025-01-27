-- テナント内にスキーマを作成します。
-- テナント追加時に実行します。

-- 実行DB:{{TenantKey}}, 実行ロール:db_admin
CREATE SCHEMA IF NOT EXISTS resultcollector;
