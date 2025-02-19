-- 検査メニュー
insert into resultcollector.exam_menus(exam_menu_id, name, order_number, enabled, created_at, created_by)
    values(2009, '血液検査', 2009, true, CURRENT_TIMESTAMP,'ec2009');
-- 検査項目グループ
insert into resultcollector.exam_item_groups(exam_item_group_id, name, exam_menu_id, type, order_number, created_at, created_by)
    values(2009, '血液検査', 2009, 1, 1, CURRENT_TIMESTAMP,'ec2009');
-- 検査項目
insert into resultcollector.exam_items(exam_item_id, name, exam_item_group_id, position_number, unit, order_number, created_at, created_by)
    values(20091, '白血球数', 2009, 1, '1000/ul',1, CURRENT_TIMESTAMP,'ec2009')
    ,     (20092, '赤血球数', 2009, 2, '1000000/ul',2, CURRENT_TIMESTAMP,'ec2009')
    ,     (20093, '血小板数', 2009, 3, '1000/ul',3, CURRENT_TIMESTAMP,'ec2009')
    ,     (20094, 'ヘモグロビン', 2009, 4, 'g/dL',4, CURRENT_TIMESTAMP,'ec2009')
    ,     (20095, 'ヘマトクリット', 2009, 5, '%',5, CURRENT_TIMESTAMP,'ec2009');
-- 検査項目明細
insert into resultcollector.exam_item_details(exam_item_detail_id, exam_item_id, name, order_number, position_number, type, keyboard_type, integer_length, decimal_length, equipment_label, created_at, created_by)
    values(20091, 20091, '白血球数', 1, 1, 1, 1, 3, 2, null, CURRENT_TIMESTAMP,'ec2009')
    ,     (20092, 20092, '赤血球数', 2, 2, 1, 1, 3, 2, null, CURRENT_TIMESTAMP,'ec2009')
    ,     (20093, 20093, '血小板数', 3, 3, 1, 1, 3, 2, null, CURRENT_TIMESTAMP,'ec2009')
    ,     (20094, 20094, 'ヘモグロビン', 4, 4, 1, 1, 3, 1, null, CURRENT_TIMESTAMP,'ec2009')
    ,     (20095, 20095, 'ヘマトクリット', 5, 5, 1, 1, 3, 1, null, CURRENT_TIMESTAMP,'ec2009');
-- 外部検査項目明細
insert into resultcollector.external_exam_item_details(exam_item_detail_id, external_exam_item_detail_code, created_at, created_by)
    values(20091, 'EC2009WBC', CURRENT_TIMESTAMP,'ec2009')
    ,     (20092, 'EC2009RBC', CURRENT_TIMESTAMP,'ec2009')
    ,     (20093, 'EC2009PLT', CURRENT_TIMESTAMP,'ec2009')
    ,     (20094, 'EC2009HGB', CURRENT_TIMESTAMP,'ec2009')
    ,     (20095, 'EC2009HCT', CURRENT_TIMESTAMP,'ec2009');
-- 基準値パターン
insert into resultcollector.thresholds(threshold_id, threshold_code, name, order_number, created_at, created_by)
    values('e0002009-0000-0000-0000-000000000001', 'EC2009', 'EC2009血液検査', 1, CURRENT_TIMESTAMP,'ec2009');
-- 検査基準値範囲
insert into resultcollector.exam_normal_value_range(name, threshold_id, exam_item_detail_id, min_age, max_age, target_sex, min_value, max_value, error_level, created_at, created_by)
    values('ヘモグロビン', 'e0002009-0000-0000-0000-000000000001', 20094, '0400000', '0750000', 3, 13.7, 16.8, 1, CURRENT_TIMESTAMP,'ec2009')
    ,     ('ヘマトクリット', 'e0002009-0000-0000-0000-000000000001', 20095, '0400000', '0750000', 3, 40.7, 50.1, 1, CURRENT_TIMESTAMP,'ec2009');
