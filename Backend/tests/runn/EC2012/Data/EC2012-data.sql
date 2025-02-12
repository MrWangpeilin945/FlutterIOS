-- 班
insert into resultcollector.teams (team_id, team_code, name, order_number, created_at, created_by)
    values('c0002012-0000-0000-0000-000000000001', 'T2012-01', 'Ⅰ班', 1, CURRENT_TIMESTAMP, 'ec2012')
    ,     ('c0002012-0000-0000-0000-000000000002', 'T2012-02', 'Ⅱ班', 2, CURRENT_TIMESTAMP, 'ec2012')
    ,     ('c0002012-0000-0000-0000-000000000003', 'T2012-03', 'Ⅲ班', 3, CURRENT_TIMESTAMP, 'ec2012')
    ,     ('c0002012-0000-0000-0000-000000000004', 'T2012-04', 'Ⅳ班', 4, CURRENT_TIMESTAMP, 'ec2012');
-- 会場
insert into resultcollector.places (place_id, place_code, name, order_number, created_at, created_by)
    values('e0002012-0000-0000-0000-000000000001', 'P2012-01', '第Ⅰ会場', 1, CURRENT_TIMESTAMP, 'ec2012')
    ,     ('e0002012-0000-0000-0000-000000000002', 'P2012-02', '第Ⅱ会場', 2, CURRENT_TIMESTAMP, 'ec2012')
    ,     ('e0002012-0000-0000-0000-000000000003', 'P2012-03', '第Ⅲ会場', 3, CURRENT_TIMESTAMP, 'ec2012')
    ,     ('e0002012-0000-0000-0000-000000000004', 'P2012-04', '第Ⅳ会場', 4, CURRENT_TIMESTAMP, 'ec2012');
-- 会場日程
insert into resultcollector.place_schedule(place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('e0002012-0000-0000-0000-000000000003', 'c0002012-0000-0000-0000-000000000003', 31, '2025/03/03', '1500', CURRENT_TIMESTAMP, 'ec2012')
    ,     ('e0002012-0000-0000-0000-000000000004', 'c0002012-0000-0000-0000-000000000004', 31, '2025/03/01', '0830', CURRENT_TIMESTAMP, 'ec2012');
