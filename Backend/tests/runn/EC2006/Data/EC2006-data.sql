-- 班   EC2006-02.ymlの実行前にレコードを作成する
insert into resultcollector.teams (team_code, name , order_number , created_at, created_by)
    values('T2006-02-1', '旧１班', 1, CURRENT_TIMESTAMP, 'ec2006')
    ,     ('T2006-02-2', '旧２班', 2, CURRENT_TIMESTAMP, 'ec2006')
    ,     ('T2006-02-3', '旧３班', 3, CURRENT_TIMESTAMP, 'ec2006');
