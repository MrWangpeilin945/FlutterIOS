-- 会場　EC2007-02.ymlの実行前にレコードを作成する
insert into resultcollector.places(place_code, name, order_number, created_at, created_by)
    values('P2007-02-1', '旧第１会場', 1, CURRENT_TIMESTAMP, 'ec2007')
    ,     ('P2007-02-2', '旧第２会場', 2, CURRENT_TIMESTAMP, 'ec2007')
    ,     ('P2007-02-3', '旧第３会場', 3, CURRENT_TIMESTAMP, 'ec2007');
   
