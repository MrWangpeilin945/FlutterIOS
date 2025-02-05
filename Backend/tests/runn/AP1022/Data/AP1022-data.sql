-- 会場
insert into resultcollector.places(place_id, place_code, name, order_number, created_at, created_by)
    values('b0001022-0000-0000-0000-000000000001', 'AP1022', '会場1022', 1022, CURRENT_TIMESTAMP,'ap1022');
-- 班
insert into resultcollector.teams(team_id, team_code, name, order_number, created_at, created_by)
    values('c0001022-0000-0000-0000-000000000001', 'AP1022', '班1022', 1022,  CURRENT_TIMESTAMP,'ap1022');
-- 会場日程を登録する
insert into resultcollector.place_schedule(place_schedule_id, place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('00001022-0000-0000-0000-000000000001','b0001022-0000-0000-0000-000000000001','c0001022-0000-0000-0000-000000000001',21,'2025-02-03','0830', CURRENT_TIMESTAMP,'ap1022');
-- 受診者を登録する
insert into resultcollector.examinees(examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
    values('e0001022-0000-0000-0000-000000000001', '1022', '両備　三郎', 'リョウビ　サブロウ', 1, '1975-10-10', CURRENT_TIMESTAMP,'ap1022'); 
-- 受診を登録する
insert into resultcollector.consult(consult_id, consult_number, progress_status, export_status, place_schedule_id, note, examinee_id, external_connection_code, created_at, created_by)
    values('a0102200-0000-0000-0000-000000000001', '1022', 11, 11, '00001022-0000-0000-0000-000000000001','来場待ち','e0001022-0000-0000-0000-000000000001','1022', CURRENT_TIMESTAMP,'ap1022');
-- 検査項目明細依頼
insert into resultcollector.exam_item_detail_orders(consult_id, exam_item_detail_id,external_exam_item_detail_code, created_at, created_by)
    values('a0102200-0000-0000-0000-000000000001', 1, '', CURRENT_TIMESTAMP, 'ap1022')
    ,    ('a0102200-0000-0000-0000-000000000001', 711, '', CURRENT_TIMESTAMP, 'ap1022')
    ,    ('a0102200-0000-0000-0000-000000000001', 712, '', CURRENT_TIMESTAMP, 'ap1022')
    ,    ('a0102200-0000-0000-0000-000000000001', 721, '', CURRENT_TIMESTAMP, 'ap1022')
    ,    ('a0102200-0000-0000-0000-000000000001', 722, '', CURRENT_TIMESTAMP, 'ap1022');
-- 中止を登録する
insert into resultcollector.exam_cancels(consult_id, exam_item_detail_id, cancel_reason_id, created_at, created_by)
    values('a0102200-0000-0000-0000-000000000001', 1, 1, CURRENT_TIMESTAMP,'ap1022')
    ,     ('a0102200-0000-0000-0000-000000000001', 721, 2, CURRENT_TIMESTAMP,'ap1022');
