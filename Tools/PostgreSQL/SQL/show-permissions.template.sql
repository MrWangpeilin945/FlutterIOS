-- テナント内のオブジェクトに付与された権限を確認します。
-- 実行DB:{{TenantKey}}, 実行ロール:db_admin

-- データベースレベルの権限確認
SELECT datname, usename, has_database_privilege(usename, datname, 'CONNECT') AS connect,
       has_database_privilege(usename, datname, 'TEMPORARY') AS temporary
FROM pg_database, pg_user
WHERE datname = '{{TenantKey}}';

-- スキーマレベルの権限確認
SELECT nspname, usename, has_schema_privilege(usename, nspname, 'CREATE') AS create,
       has_schema_privilege(usename, nspname, 'USAGE') AS usage
FROM pg_namespace, pg_user
WHERE nspname = 'resultcollector';

-- テーブルレベルの権限確認
SELECT tablename, usename, has_table_privilege(usename, tablename, 'SELECT') AS select,
       has_table_privilege(usename, tablename, 'INSERT') AS insert,
       has_table_privilege(usename, tablename, 'UPDATE') AS update,
       has_table_privilege(usename, tablename, 'DELETE') AS delete,
       has_table_privilege(usename, tablename, 'TRUNCATE') AS truncate
FROM pg_tables, pg_user
WHERE schemaname = 'resultcollector';
