-- 班   EC2006-02.ymlの実行前にレコードを作成する
insert into resultcollector.teams (team_code, name , order_number , created_at, created_by)
    values('T241001', '旧１班', 1, CURRENT_TIMESTAMP, 'ec2006')
    ,     ('T241002', '旧２班', 2, CURRENT_TIMESTAMP, 'ec2006');
