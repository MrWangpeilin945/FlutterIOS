-- 会場
insert into resultcollector.places(place_id, place_code, name, order_number, created_at, created_by)
    values('b0001010-0000-0000-0000-000000000001', 'AP1010', '会場1010', 1010, CURRENT_TIMESTAMP,'ap1010');
-- 班
insert into resultcollector.teams(team_id, team_code, name, order_number, created_at, created_by)
    values('c0001010-0000-0000-0000-000000000001', 'AP1010', '班1010', 1010,  CURRENT_TIMESTAMP,'ap1010');
-- 会場日程を登録する
insert into resultcollector.place_schedule(place_schedule_id, place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('00001010-0000-0000-0000-000000000001','b0001010-0000-0000-0000-000000000001','c0001010-0000-0000-0000-000000000001',21,'2025-03-31','1030', CURRENT_TIMESTAMP,'ap1010');
-- 受診者を登録する
insert into resultcollector.examinees(examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
    values('e0001010-0000-0000-0000-000000000001', '10101', '同姓　薫', 'ドウセイ　カオル', 1, '1991-01-30', CURRENT_TIMESTAMP,'ap1010')
    ,     ('e0001010-0000-0000-0000-000000000002', '10102', '同姓　香', 'ドウセイ　カオル', 2, '1989-03-15', CURRENT_TIMESTAMP,'ap1010')
    ,     ('e0001010-0000-0000-0000-000000000003', '10103', '同姓　薫', 'ドウセイ　カヲル', 2, '1989-03-15', CURRENT_TIMESTAMP,'ap1010');
-- 受診を登録する
insert into resultcollector.consult(consult_id, consult_number, progress_status, export_status, place_schedule_id, note, examinee_id, external_connection_code, created_at, created_by)
    values('a0101000-0000-0000-0000-000000000001', '10101', 11, 11, '00001010-0000-0000-0000-000000000001','','e0001010-0000-0000-0000-000000000001','10141', CURRENT_TIMESTAMP,'ap1014')
    ,     ('a0101000-0000-0000-0000-000000000002', '10102', 11, 11, '00001010-0000-0000-0000-000000000001','','e0001010-0000-0000-0000-000000000002','10142', CURRENT_TIMESTAMP,'ap1014')
    ,     ('a0101000-0000-0000-0000-000000000003', '10103', 11, 11, '00001010-0000-0000-0000-000000000001','','e0001010-0000-0000-0000-000000000003','10143', CURRENT_TIMESTAMP,'ap1014');
