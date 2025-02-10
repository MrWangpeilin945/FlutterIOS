-- 会場
insert into resultcollector.places(place_id, place_code, name, order_number, created_at, created_by)
    values('b0001014-0000-0000-0000-000000000001', 'AP1014', '会場1014', 1014, CURRENT_TIMESTAMP,'ap1014');
-- 班
insert into resultcollector.teams(team_id, team_code, name, order_number, created_at, created_by)
    values('c0001014-0000-0000-0000-000000000001', 'AP1014', '班1014', 1014,  CURRENT_TIMESTAMP,'ap1014');
-- 会場日程を登録する
insert into resultcollector.place_schedule(place_schedule_id, place_id, team_id, status, exam_date, start_time, created_at, created_by)
    values('00001014-0000-0000-0000-000000000001','b0001014-0000-0000-0000-000000000001','c0001014-0000-0000-0000-000000000001',21,'2025-02-10','1000', CURRENT_TIMESTAMP,'ap1014')
    ,     ('00001014-0000-0000-0000-000000000002','b0001014-0000-0000-0000-000000000001','c0001014-0000-0000-0000-000000000001',31,'2025-02-20','1000', CURRENT_TIMESTAMP,'ap1014');
-- 受診者を登録する
insert into resultcollector.examinees(examinee_id, examinee_code, name, kana_name, sex, birthdate, created_at, created_by)
    values('e0001014-0000-0000-0000-000000000001', '1014', '両備　四郎', 'リョウビ　シロウ', 1, '1973-06-30', CURRENT_TIMESTAMP,'ap1014');
-- 受診を登録する
insert into resultcollector.consult(consult_id, consult_number, progress_status, export_status, place_schedule_id, note, examinee_id, external_connection_code, created_at, created_by)
    values('a0101400-0000-0000-0000-000000000001', '1014', 11, 11, '00001014-0000-0000-0000-000000000001','','e0001014-0000-0000-0000-000000000001','1014', CURRENT_TIMESTAMP,'ap1014')
    ,     ('a0101400-0000-0000-0000-000000000002', '1114', 11, 11, '00001014-0000-0000-0000-000000000002','','e0001014-0000-0000-0000-000000000001','1114', CURRENT_TIMESTAMP,'ap1014');
-- 検査メニュー
insert into resultcollector.exam_menus(exam_menu_id, name, order_number,enabled, created_at, created_by)
    values(1014, '脂質検査', 101, true, CURRENT_TIMESTAMP, 'ap1014');
-- 検査項目グループ
insert into resultcollector.exam_item_groups(exam_item_group_id, name, exam_menu_id, type, order_number, created_at, created_by)
    values(1014, '脂質検査', 1014, 1, 102, CURRENT_TIMESTAMP, 'ap1014');
-- 検査項目
insert into resultcollector.exam_items(exam_item_id, name, exam_item_group_id, position_number, unit, order_number, created_at, created_by)
    values(10141, '中性脂肪', 1014, 1, 'mg/dL', 1, CURRENT_TIMESTAMP, 'ap1014')
    ,     (10142, '総コレステロール', 1014, 2, 'mg/dL', 2, CURRENT_TIMESTAMP, 'ap1014');
-- 検査項目明細
insert into resultcollector.exam_item_details(exam_item_detail_id, exam_item_id, name, order_number, position_number, type, keyboard_type, integer_length, decimal_length, equipment_label, created_at, created_by)
    values(10141, 10141, '中性脂肪', 1, 1, 1, 1, 3, 0, null, CURRENT_TIMESTAMP, 'ap1014')
    ,     (10142, 10142, '総コレステロール', 2, 2, 1, 1, 3, 0, null, CURRENT_TIMESTAMP, 'ap1014'); 
-- 職員権限
update resultcollector.staffs set role_id = 10 where staff_code='S001';
update resultcollector.staffs set role_id = 20 where staff_code='S002';
