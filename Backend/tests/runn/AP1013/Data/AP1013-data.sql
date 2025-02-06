-- 検査メニュー
insert into resultcollector.exam_menus(exam_menu_id, name, order_number,enabled, created_at, created_by)
    values(1013, '眼科', 101, true, CURRENT_TIMESTAMP, 'ap1013');
-- 検査項目グループ
insert into resultcollector.exam_item_groups(exam_item_group_id, name, exam_menu_id, type, order_number, created_at, created_by)
    values(1013, '眼科', 1013, 1, 101, CURRENT_TIMESTAMP, 'ap1013');
-- 検査項目
insert into resultcollector.exam_items(exam_item_id, name, exam_item_group_id, position_number, unit, order_number, created_at, created_by)
    values(10131, '眼底検査', 1013, 1, '', 1, CURRENT_TIMESTAMP, 'ap1013')
    ,     (10132, '眼圧検査', 1013, 2, 'mmHg', 2, CURRENT_TIMESTAMP, 'ap1013');
-- 検査項目明細
insert into resultcollector.exam_item_details(exam_item_detail_id, exam_item_id, name, order_number, position_number, type, keyboard_type, integer_length, decimal_length, equipment_label, created_at, created_by)
    values(101311, 10131, '眼底右', 1, 1, 1, 1, 3, 0, null, CURRENT_TIMESTAMP, 'ap1013')
    ,     (101312, 10131, '眼底左', 2, 2, 1, 1, 3, 0, null, CURRENT_TIMESTAMP, 'ap1013') 
    ,     (101321, 10132, '眼圧左', 3, 3, 1, 1, 3, 0, null, CURRENT_TIMESTAMP, 'ap1013') 
    ,     (101322, 10132, '眼圧左', 4, 4, 1, 1, 3, 0, null, CURRENT_TIMESTAMP, 'ap1013');
-- 会場
insert into resultcollector.places(place_id, place_code, name, order_number, created_at, created_by)
    values('b0001013-0000-0000-0000-000000000001', 'AP1013', '会場1013', 1013, CURRENT_TIMESTAMP, 'ap1013');
-- 班
insert into resultcollector.teams(team_id, team_code, name, order_number, created_at, created_by)
    values('c0001013-0000-0000-0000-000000000001', 'AP1013', '班1013', 1013,  CURRENT_TIMESTAMP, 'ap1013');
-- 会場日程を登録する
insert into resultcollector.place_schedule(place_schedule_id, place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('00001013-0000-0000-0000-000000000001','b0001013-0000-0000-0000-000000000001','c0001013-0000-0000-0000-000000000001',21,'2025-02-10','1000', CURRENT_TIMESTAMP,'ap1013');
-- 受診者を登録する
insert into resultcollector.examinees(examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
    values('e0001013-0000-0000-0000-000000000001', '1013', '両備　十三', 'リョウビ　ジュウゾウ', 1, '1984-12-30', CURRENT_TIMESTAMP,'ap1013');
-- 受診を登録する
insert into resultcollector.consult(consult_id, consult_number, progress_status, export_status, place_schedule_id, note, examinee_id, external_connection_code, created_at, created_by)
    values('a0001013-0000-0000-0000-000000000001', '1013', 21, 31, '00001013-0000-0000-0000-000000000001','','e0001013-0000-0000-0000-000000000001','1013', CURRENT_TIMESTAMP,'ap1013');
-- 検査結果
insert into resultcollector.exam_results(consult_id, exam_item_detail_id, value, created_at, created_by)
    values('a0001013-0000-0000-0000-000000000001', 101311, '2', CURRENT_TIMESTAMP, 'ap1013')
    ,     ('a0001013-0000-0000-0000-000000000001', 101312, '1', CURRENT_TIMESTAMP, 'ap1013')
    ,     ('a0001013-0000-0000-0000-000000000001', 101321, '25', CURRENT_TIMESTAMP, 'ap1013')
    ,     ('a0001013-0000-0000-0000-000000000001', 101322, '24', CURRENT_TIMESTAMP, 'ap1013');
-- 過去検査結果
insert into resultcollector.previous_results(consult_id, exam_date, exam_item_detail_id, value, created_at, created_by)
    values('a0001013-0000-0000-0000-000000000001', '2023-09-23', 101311, '1', CURRENT_TIMESTAMP, 'ap1013')
    ,     ('a0001013-0000-0000-0000-000000000001', '2023-09-23',101312, '1', CURRENT_TIMESTAMP, 'ap1013')
    ,     ('a0001013-0000-0000-0000-000000000001', '2023-09-23',101321, '23', CURRENT_TIMESTAMP, 'ap1013')
    ,     ('a0001013-0000-0000-0000-000000000001', '2023-09-23',101322, '26', CURRENT_TIMESTAMP, 'ap1013');
-- 検査結果相関ルール
insert into resultcollector.correlation_rules(correlation_rule_id,name,exam_menu_id,priority,trigger_type,error_level,exam_item_id, message,created_at,created_by)
    values(1013,'入力値範囲外', 1013, 1, 1, 3, 10131, '入力値範囲外です。', CURRENT_TIMESTAMP, 'ap1013');
-- 検査結果相関ルール_検査項目明細
insert into resultcollector.correlation_rule_exam_item_details(correlation_rule_id,variable_number,source_type,exam_item_detail_id,created_at,created_by)
    values (1013, 1, 1, 101311, CURRENT_TIMESTAMP,'ap1013')
    ,      (1013, 2, 2, 101311, CURRENT_TIMESTAMP,'ap1013');
-- 検査結果相関ルール_判定値
insert into resultcollector.correlation_rule_evaluations(correlation_rule_id,variable_number,evaluation_value,created_at,created_by)
    values(1013, 1, '10', CURRENT_TIMESTAMP, 'ap1013');
