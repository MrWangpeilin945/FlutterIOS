-- 会場
insert into resultcollector.places(place_id, place_code, name, order_number, created_at, created_by)
    values('b0001014-0000-0000-0000-000000000001', 'AP1014', '会場1014', 1014, CURRENT_TIMESTAMP,'ap1014');
-- 班
insert into resultcollector.teams(team_id, team_code, name, order_number, created_at, created_by)
    values('c0001014-0000-0000-0000-000000000001', 'AP1014', '班1014', 1014,  CURRENT_TIMESTAMP,'ap1014');
-- 会場日程を登録する
insert into resultcollector.place_schedule(place_schedule_id, place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('00001014-0000-0000-0000-000000000001','b0001014-0000-0000-0000-000000000001','c0001014-0000-0000-0000-000000000001',21,'2025-02-10','1000', CURRENT_TIMESTAMP,'ap1014');
-- 受診者を登録する
insert into resultcollector.examinees(examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
    values('e0001014-0000-0000-0000-000000000001', '1014', '両備　四郎', 'リョウビ　シロウ', 1, '1973-06-30', CURRENT_TIMESTAMP,'ap1014');
-- 受診を登録する
insert into resultcollector.consult(consult_id, consult_number, progress_status, export_status, place_schedule_id, note, examinee_id, external_connection_code, created_at, created_by)
    values('a0101400-0000-0000-0000-000000000001', '1014', 11, 11, '00001014-0000-0000-0000-000000000001','','e0001014-0000-0000-0000-000000000001','1014', CURRENT_TIMESTAMP,'ap1014');
