-- 会場
insert into resultcollector.places(place_id, place_code, name, order_number, created_at, created_by)
    values('b0001021-0000-0000-0000-000000000001', 'AP1021', '会場1021', 1021, CURRENT_TIMESTAMP,'ap1021');
-- 班
insert into resultcollector.teams(team_id, team_code, name, order_number, created_at, created_by)
    values('c0001021-0000-0000-0000-000000000001', 'AP1021', '班1021', 1021,  CURRENT_TIMESTAMP,'ap1021');
-- 会場日程を登録する
insert into resultcollector.place_schedule(place_schedule_id, place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('00001021-0000-0000-0000-000000000001','b0001021-0000-0000-0000-000000000001','c0001021-0000-0000-0000-000000000001',21,'2025-02-10','1000', CURRENT_TIMESTAMP,'ap1021');
-- 受診者を登録する
insert into resultcollector.examinees(examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
    values('e0001021-0000-0000-0000-000000000001', '1021', '両備　六郎', 'リョウビ　ロクロウ', 1, '1981-12-30', CURRENT_TIMESTAMP,'ap1021');
-- 受診を登録する
insert into resultcollector.consult(consult_id, consult_number, progress_status, export_status, place_schedule_id, note, examinee_id, external_connection_code, created_at, created_by)
    values('a0001021-0000-0000-0000-000000000001', '1021', 21, 31, '00001021-0000-0000-0000-000000000001','','e0001021-0000-0000-0000-000000000001','1021', CURRENT_TIMESTAMP,'ap1021');
-- 検査結果出力履歴
insert into resultcollector.export_histories(id, place_schedule_id, exported_at, exported_by, created_at, created_by)
    values('f0001021-0000-0000-0000-000000000001', '00001021-0000-0000-0000-000000000001', CURRENT_TIMESTAMP, 'テスト職員1021', CURRENT_TIMESTAMP, 'ap1021');
-- export_history_details
insert into resultcollector.export_history_details(id, consult_id, created_at, created_by)
    values('f0001021-0000-0000-0000-000000000001', 'a0001021-0000-0000-0000-000000000001', CURRENT_TIMESTAMP,'ap1021');
