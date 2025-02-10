-- 会場　EC2007-02.ymlの実行前にレコードを作成する
insert into resultcollector.places(place_code, name, order_number, created_at, created_by)
    values('P241001', '旧第１会場', 1, CURRENT_TIMESTAMP, 'ec2007')
    ,     ('P241002', '旧第２会場', 2, CURRENT_TIMESTAMP, 'ec2007');