-- テナント内のオブジェクトに権限を付与します。
-- テナント追加時に実行します。

-- 実行DB:{{TenantKey}}, 実行ロール:db_admin

-- データベース単位
GRANT CONNECT, TEMPORARY ON DATABASE {{TenantKey}} TO app, {{TenantKey}}_ops;

-- スキーマ単位
REVOKE USAGE, CREATE ON SCHEMA resultcollector FROM PUBLIC;
GRANT CREATE, USAGE ON SCHEMA resultcollector TO app, {{TenantKey}}_ops;

-- テーブル単位
ALTER DEFAULT PRIVILEGES IN SCHEMA resultcollector GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO app, {{TenantKey}}_ops;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA resultcollector TO app, {{TenantKey}}_ops;
GRANT TRUNCATE ON ALL TABLES IN SCHEMA resultcollector TO {{TenantKey}}_ops;
