-- DB切り替え
\c wellship

set search_path = resultcollector;

-- 受診者
INSERT INTO examinees(examinee_id,examinee_code,name,kana_name,sex,birthdate,created_at,created_by) VALUES 
    ('eaee0000-0000-0000-0000-000000000001','100001','両備　花子001','リョウビ　ハナコ001',2,DATE '2012-11-19',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000002','100002','両備　花子002','リョウビ　ハナコ002',2,DATE '1939-03-16',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000003','100003','両備　花子003','リョウビ　ハナコ003',2,DATE '2001-04-29',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000004','100004','両備　花子004','リョウビ　ハナコ004',2,DATE '1986-02-02',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000005','100005','両備　花子005','リョウビ　ハナコ005',2,DATE '1962-09-21',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000006','100006','両備　花子006','リョウビ　ハナコ006',2,DATE '1942-05-16',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000007','100007','両備　花子007','リョウビ　ハナコ007',2,DATE '1976-02-11',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000008','100008','両備　花子008','リョウビ　ハナコ008',2,DATE '2006-05-20',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000009','100009','両備　花子009','リョウビ　ハナコ009',2,DATE '1976-07-01',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000010','100010','両備　花子010','リョウビ　ハナコ010',2,DATE '1952-01-21',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000011','100011','両備　太郎011','リョウビ　タロウ011',1,DATE '1957-05-25',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000012','100012','両備　太郎012','リョウビ　タロウ012',1,DATE '2024-11-12',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000013','100013','両備　太郎013','リョウビ　タロウ013',1,DATE '2002-05-11',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000014','100014','両備　太郎014','リョウビ　タロウ014',1,DATE '1991-06-07',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000015','100015','両備　太郎015','リョウビ　タロウ015',1,DATE '1999-01-05',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000016','100016','両備　太郎016','リョウビ　タロウ016',1,DATE '1999-05-15',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000017','100017','両備　太郎017','リョウビ　タロウ017',1,DATE '1924-04-01',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000018','100018','両備　太郎018','リョウビ　タロウ018',1,DATE '1957-06-16',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000019','100019','両備　太郎019','リョウビ　タロウ019',1,DATE '1982-09-18',CURRENT_TIMESTAMP,'init')
  , ('eaee0000-0000-0000-0000-000000000020','100020','両備　太郎020','リョウビ　タロウ020',1,DATE '1926-03-25',CURRENT_TIMESTAMP,'init');

-- 会場
INSERT INTO places(place_id,place_code,name,order_number,created_at,created_by) VALUES 
    ('aced0000-0000-0000-0000-000000000001','P001','会場A',1,CURRENT_TIMESTAMP,'init')
  , ('aced0000-0000-0000-0000-000000000002','P002','会場B',2,CURRENT_TIMESTAMP,'init')
  , ('aced0000-0000-0000-0000-000000000003','P003','会場C',3,CURRENT_TIMESTAMP,'init')
  , ('aced0000-0000-0000-0000-000000000004','P004','会場D',4,CURRENT_TIMESTAMP,'init')
  , ('aced0000-0000-0000-0000-000000000005','P005','会場E',5,CURRENT_TIMESTAMP,'init');

-- 班
INSERT INTO teams(team_id,team_code,name,order_number,created_at,created_by) VALUES 
    ('ea000000-0000-0000-0000-000000000001','T001','班A',1,CURRENT_TIMESTAMP,'init')
  , ('ea000000-0000-0000-0000-000000000002','T002','班B',2,CURRENT_TIMESTAMP,'init')
  , ('ea000000-0000-0000-0000-000000000003','T003','班C',3,CURRENT_TIMESTAMP,'init');

-- 会場日程
INSERT INTO place_schedule(place_schedule_id,place_id,team_id,status,exam_date,start_time,created_at,created_by) VALUES 
    ('aceced00-0000-0000-0000-000000000001','aced0000-0000-0000-0000-000000000001','ea000000-0000-0000-0000-000000000001',21,DATE '2024-10-01','0900',CURRENT_TIMESTAMP,'S001')
  , ('aceced00-0000-0000-0000-000000000002','aced0000-0000-0000-0000-000000000001','ea000000-0000-0000-0000-000000000001',21,DATE '2024-10-02','1300',CURRENT_TIMESTAMP,'init')
  , ('aceced00-0000-0000-0000-000000000003','aced0000-0000-0000-0000-000000000001','ea000000-0000-0000-0000-000000000002',21,DATE '2024-11-02','1000',CURRENT_TIMESTAMP,'init')
  , ('20d6daa6-eac7-4ef4-8ea4-0f3b439668a6','aced0000-0000-0000-0000-000000000001','ea000000-0000-0000-0000-000000000001',21,DATE '2024-11-30','1000',CURRENT_TIMESTAMP,'init');

-- 検査メニュー
INSERT INTO exam_menus(exam_menu_id,name,order_number,enabled,created_at,created_by) VALUES 
    (1,'身体計測',1,True,CURRENT_TIMESTAMP,'init')
  , (2,'視力',2,True,CURRENT_TIMESTAMP,'init')
  , (3,'聴力',3,True,CURRENT_TIMESTAMP,'init')
  , (4,'心電図',4,True,CURRENT_TIMESTAMP,'init')
  , (5,'腹囲',5,True,CURRENT_TIMESTAMP,'init')
  , (6,'眼底',6,True,CURRENT_TIMESTAMP,'init')
  , (7,'血圧',7,True,CURRENT_TIMESTAMP,'init')
  , (8,'眼圧',8,True,CURRENT_TIMESTAMP,'init')
  , (9,'握力',9,True,CURRENT_TIMESTAMP,'init');

-- 検査項目グループ
INSERT INTO exam_item_groups(exam_item_group_id,name,exam_menu_id,type,order_number,created_at,created_by) VALUES 
    (1,'身体計測',1,1,1,CURRENT_TIMESTAMP,'init')
  , (7,'血圧',7,7,2,CURRENT_TIMESTAMP,'init');

-- 検査項目
INSERT INTO exam_items(exam_item_id,name,exam_item_group_id,position_number,unit,order_number,created_at,created_by) VALUES 
    (1,'身長',1,1,'cm',1,CURRENT_TIMESTAMP,'init')
  , (2,'体重',1,1,'kg',2,CURRENT_TIMESTAMP,'init')
  , (71,'血圧1',7,1,null,1,CURRENT_TIMESTAMP,'init')
  , (72,'血圧2',7,2,null,2,CURRENT_TIMESTAMP,'init');

-- 検査項目明細
INSERT INTO exam_item_details(exam_item_detail_id,exam_item_id,name,order_number,position_number,type,keyboard_type,integer_length,decimal_length,equipment_label,created_at,created_by) VALUES 
    (1,1,'身長',1,1,1,1,3,2,null,CURRENT_TIMESTAMP,'init')
  , (2,2,'体重',1,1,1,1,3,2,null,CURRENT_TIMESTAMP,'init')
  , (711,71,'血圧1_上',1,1,1,1,3,0,null,CURRENT_TIMESTAMP,'init')
  , (712,71,'血圧1_下',2,2,1,1,3,0,null,CURRENT_TIMESTAMP,'init')
  , (721,72,'血圧2_上',1,1,1,1,3,0,null,CURRENT_TIMESTAMP,'init')
  , (722,72,'血圧2_下',2,2,1,1,3,0,null,CURRENT_TIMESTAMP,'init');

-- ホームメニューグループ
INSERT INTO home_menu_groups(home_menu_group_id,name,order_number,created_at,created_by) VALUES 
    (1,'一般',1,CURRENT_TIMESTAMP,'init')
  , (2,'管理',2,CURRENT_TIMESTAMP,'init');

-- ホームメニュー
INSERT INTO home_menus(home_menu_id,name,home_menu_group_id,order_number,path,created_at,created_by) VALUES 
    (1,'検査メニュー選択',1,1,'exammenu-select',CURRENT_TIMESTAMP,'init')
  , (2,'進捗',1,2,'progress',CURRENT_TIMESTAMP,'init')
  , (3,'会場ロック',2,1,'placeschedule-lock',CURRENT_TIMESTAMP,'init')
  , (4,'検査結果出力',2,2,'examresult-export',CURRENT_TIMESTAMP,'init')
  , (5,'検査結果出力履歴',2,3,'examresult-export-history',CURRENT_TIMESTAMP,'init');

-- 受診
INSERT INTO consult(consult_id,consult_number,age,progress_status,export_status,place_schedule_id,note,examinee_id,external_connection_code,created_at,created_by) VALUES 
    ('caaaaa00-0000-0000-0000-000000000001','0001','0400910',21,11,'aceced00-0000-0000-0000-000000000002','','eaee0000-0000-0000-0000-000000000002','10001',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002','0002','0400910',21,11,'aceced00-0000-0000-0000-000000000002','','eaee0000-0000-0000-0000-000000000012','10002',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003','0003','0711129',41,11,'aceced00-0000-0000-0000-000000000001','','eaee0000-0000-0000-0000-000000000018','10003',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004','0004','0400910',51,21,'aceced00-0000-0000-0000-000000000002','','eaee0000-0000-0000-0000-000000000013','10004',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000005','0005','0711129',41,31,'aceced00-0000-0000-0000-000000000001','','eaee0000-0000-0000-0000-000000000014','10005',CURRENT_TIMESTAMP,'init')
  , ('8dda2a54-5217-425f-bba9-ab821a9647fe','0006','0250115',21,11,'20d6daa6-eac7-4ef4-8ea4-0f3b439668a6','定期健康診断','eaee0000-0000-0000-0000-000000000020','1002',CURRENT_TIMESTAMP,'init');

-- ロール
INSERT INTO roles(role_id,name,created_at,created_by) VALUES 
    (10,'一般',CURRENT_TIMESTAMP,'init')
  , (20,'管理者',CURRENT_TIMESTAMP,'init');

-- 職員
INSERT INTO staffs(staff_id,staff_code,login_id,name,password_hash,password_salt,enabled,role_id,created_at,created_by) VALUES 
    -- 職員Aパスワード：syokuinA
    ('affd0000-0000-0000-0000-000000000001','S001','S001','職員A',DECODE('TX+Y1tzM7x6bfFxQob8oVpkSfY+avT+MJpGRzzJ54yilcsTx1T987plGIUW7ORJhfcPAPqreEzyyjzq2ufsw+w==', 'base64'),DECODE('NaGPpMzTwdKiSWC8zWjj84nKm8WlXvlZSwIJ4o7kjo9/j4Hllkk61/8Vz14JVAx/KGt3GMGNE0Z/LmEb7Qfe4fAjf+aVMNkyuAywRBzwT7hUbzivt3NHohOVgIg3tYnVdLn+M4sORWIiYmq5kot9zWc02rRFeSraF4jORxQXucQ=', 'base64'),True,10,CURRENT_TIMESTAMP,'init')
    -- 職員Bパスワード：syokuinB
  , ('affd0000-0000-0000-0000-000000000002','S002','S002','職員B',DECODE('yQTwvk3Ikc+VmAq6nFXfoXeDbNR1RGOnuK62e5NLz6quuPp/5mtxBFZLxDLqngERjYu7AyPAO2FhW/R0e7oxyw==', 'base64'),DECODE('q0T+ID2EsZ5blalDOD1Jweorjd/z+g4f1yChM/2+OcEBdvZikMyVYIDURkklt5W3JtCP41bpb6rWl6MnL+R6naTj0a4CqRkToVU6GXhhMNlAHbei8RBDHSWi9fSX5+c68pJaYflvndTCaUQAO/0jDpwW9rzE0jBzlx7H2kZzPMc=', 'base64'),True,20,CURRENT_TIMESTAMP,'init');

-- 団体
INSERT INTO organizations(organization_id,organization_code,name,order_number,created_at,created_by) VALUES 
    ('aad00000-0000-0000-0000-000000000001','11','株式会社テストA',1,CURRENT_TIMESTAMP,'init')
  , ('aad00000-0000-0000-0000-000000000002','12','株式会社テストB',2,CURRENT_TIMESTAMP,'init')
  , ('aad00000-0000-0000-0000-000000000003','13','株式会社テストC',3,CURRENT_TIMESTAMP,'init')
  , ('aad00000-0000-0000-0000-000000000004','21','テストD株式会社',4,CURRENT_TIMESTAMP,'init')
  , ('aad00000-0000-0000-0000-000000000005','22','テストE株式会社',5,CURRENT_TIMESTAMP,'init')
  , ('aad00000-0000-0000-0000-000000000006','23','一般社団法人テストF',6,CURRENT_TIMESTAMP,'init');

-- 検査機器
INSERT INTO equipments(equipment_id,name,exam_menu_id,app_launch_url,processing_script_url,created_at,created_by) VALUES 
    (1,'EQ001',1,'wsc://abcde','https://example.com/sctipts/eq001.js',CURRENT_TIMESTAMP,'init')
  , (2,'EQ002',1,'wsc://abcde','https://example.com/sctipts/eq002.js',CURRENT_TIMESTAMP,'init')
  , (3,'EQ003',2,'wsc://abcde','https://example.com/sctipts/eq003.js',CURRENT_TIMESTAMP,'init')
  , (4,'EQ004',3,'wsc://abcde','https://example.com/sctipts/eq004.js',CURRENT_TIMESTAMP,'init')
  , (5,'EQ005',5,'wsc://abcde','https://example.com/sctipts/eq005.js',CURRENT_TIMESTAMP,'init');

-- 検査結果出力履歴
INSERT INTO export_histories(id, place_schedule_id,exported_at,exported_by,created_at,created_by) VALUES 
    ('af0971a0-186f-4b14-86a1-0c648d77c9af','aceced00-0000-0000-0000-000000000002',TIMESTAMP '2024-11-25 12:57:15.506','職員B',CURRENT_TIMESTAMP,'init')
  , ('41293e22-4290-4f85-9179-9affda055cdd','aceced00-0000-0000-0000-000000000003',TIMESTAMP '2024-10-22 20:00:00.000','職員A',CURRENT_TIMESTAMP,'init')
  , ('b53cc211-1b6a-4d20-8bb0-3c2a007661c6','aceced00-0000-0000-0000-000000000001',TIMESTAMP '2024-10-03 13:20:00.000','職員B',CURRENT_TIMESTAMP,'init')
  , ('8f2edd5a-9ad3-4445-96a8-1b2129f60929','aceced00-0000-0000-0000-000000000001',TIMESTAMP '2023-02-25 09:24:000.000','職員A',CURRENT_TIMESTAMP,'init');

-- 検査結果出力履歴明細
INSERT INTO export_history_details(id,consult_id,created_at,created_by) VALUES 
    ('af0971a0-186f-4b14-86a1-0c648d77c9af','caaaaa00-0000-0000-0000-000000000001',CURRENT_TIMESTAMP,'init')
  , ('af0971a0-186f-4b14-86a1-0c648d77c9af','caaaaa00-0000-0000-0000-000000000002',CURRENT_TIMESTAMP,'init')
  , ('af0971a0-186f-4b14-86a1-0c648d77c9af','caaaaa00-0000-0000-0000-000000000004',CURRENT_TIMESTAMP,'init')
  , ('b53cc211-1b6a-4d20-8bb0-3c2a007661c6','caaaaa00-0000-0000-0000-000000000003',CURRENT_TIMESTAMP,'init')
  , ('8f2edd5a-9ad3-4445-96a8-1b2129f60929','caaaaa00-0000-0000-0000-000000000005',CURRENT_TIMESTAMP,'init');

-- 検査項目明細依頼
INSERT INTO exam_item_detail_orders(consult_id,exam_item_detail_id,created_at,created_by) VALUES 
    ('caaaaa00-0000-0000-0000-000000000001',1,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',2,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',711,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',712,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',721,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',722,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',1,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',2,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',711,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',712,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',721,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',722,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',1,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',2,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',711,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',712,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',1,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',2,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',711,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',712,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',721,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',722,CURRENT_TIMESTAMP,'init');

-- 中止理由
INSERT INTO cancel_reasons(cancel_reason_id,name,exam_item_id,order_number,created_at,created_by) VALUES 
    (1,'1_身体的理由',1,1,CURRENT_TIMESTAMP,'init')
  , (2,'2_身体的理由',2,1,CURRENT_TIMESTAMP,'init')
  , (3,'71_理由A',71,2,CURRENT_TIMESTAMP,'init')
  , (4,'71_理由B',71,2,CURRENT_TIMESTAMP,'init')
  , (5,'71_理由C',71,2,CURRENT_TIMESTAMP,'init')
  , (6,'71_理由D',71,2,CURRENT_TIMESTAMP,'init')
  , (7,'72_理由A',72,2,CURRENT_TIMESTAMP,'init')
  , (8,'72_理由B',72,2,CURRENT_TIMESTAMP,'init')
  , (9,'72_理由C',72,2,CURRENT_TIMESTAMP,'init')
  , (10,'72_理由D',72,3,CURRENT_TIMESTAMP,'init');

-- 検査結果
INSERT INTO exam_results(consult_id,exam_item_detail_id,value,created_at,created_by) VALUES 
    ('caaaaa00-0000-0000-0000-000000000001',1,'178',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',2,'70',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',711,'98',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',712,'72',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',721,'100',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000001',722,'70',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',1,'160',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',711,'72',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',721,'100',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000002',722,'70',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',1,'180',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',2,'90',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',711,'128',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000003',712,'92',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',711,'128',CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',712,'92',CURRENT_TIMESTAMP,'init')
  , ('8dda2a54-5217-425f-bba9-ab821a9647fe',2,'160.5',CURRENT_TIMESTAMP,'init')
  , ('8dda2a54-5217-425f-bba9-ab821a9647fe',1,'175.3',CURRENT_TIMESTAMP,'init');

-- 過去検査結果
INSERT INTO previous_results(consult_id,exam_date,exam_item_detail_id,value,created_at,created_by) VALUES 
    ('8dda2a54-5217-425f-bba9-ab821a9647fe', DATE '2023-10-01',2,'70.0',CURRENT_TIMESTAMP,'init')
  , ('8dda2a54-5217-425f-bba9-ab821a9647fe', DATE '2023-10-1',1,'173.0',CURRENT_TIMESTAMP,'init');

-- 検査中止
INSERT INTO exam_cancels(consult_id,exam_item_detail_id,cancel_reason_id,created_at,created_by) VALUES 
    ('caaaaa00-0000-0000-0000-000000000004',1,1,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',2,2,CURRENT_TIMESTAMP,'init')
  , ('caaaaa00-0000-0000-0000-000000000004',721,10,CURRENT_TIMESTAMP,'init');

-- 前提検査メニュー
INSERT INTO resultcollector.prior_exam_menus(current_exam_menu_id,prior_exam_menu_id,created_at,created_by) VALUES 
    (5,2,CURRENT_TIMESTAMP,'init')
  , (5,3,CURRENT_TIMESTAMP,'init')
  , (5,4,CURRENT_TIMESTAMP,'init')
  , (6,1,CURRENT_TIMESTAMP,'init')
  , (6,2,CURRENT_TIMESTAMP,'init')
  , (7,8,CURRENT_TIMESTAMP,'init');

-- 検査結果相関ルール
INSERT INTO correlation_rules(correlation_rule_id,name,exam_menu_id,priority,trigger_type,error_level,exam_item_id, message,created_at,created_by) VALUES 
    (1,'体重_前回差20kg以上',1,1,1,2,2,'体重の前回差が20kg以上です。',CURRENT_TIMESTAMP,'init')
  , (2,'値が一部でも異なる場合はエラー',1,2,2,3,7,'登録する値が異なります。',CURRENT_TIMESTAMP,'init');

-- 検査結果相関ルール_判定値
INSERT INTO correlation_rule_evaluations(correlation_rule_id,variable_number,evaluation_value,created_at,created_by) VALUES 
    (1,1,'20.0',CURRENT_TIMESTAMP,'init');

-- 検査結果相関ルール_検査項目明細
INSERT INTO correlation_rule_exam_item_details(correlation_rule_id,variable_number,source_type,exam_item_detail_id,created_at,created_by) VALUES
    (1,1,1,2,CURRENT_TIMESTAMP,'init')
  , (1,2,2,2,CURRENT_TIMESTAMP,'init')
  , (2,1,1,711,CURRENT_TIMESTAMP,'init')
  , (2,2,1,712,CURRENT_TIMESTAMP,'init')
  , (2,3,1,721,CURRENT_TIMESTAMP,'init')
  , (2,4,1,722,CURRENT_TIMESTAMP,'init');

-- 検査メニュー特記
insert into exam_menu_notes(menu_note_id,name,exam_menu_id,order_number,suffix,created_at,created_by) VALUES
    (1,'身長',1,1,'cm',CURRENT_TIMESTAMP, 'init')
  , (2,'撮影番号/○○番号',7,2,'番',CURRENT_TIMESTAMP, 'init');

-- 検査メニュー特記_検査結果
insert into exam_menu_note_results(menu_note_id,exam_item_detail_id,source_type,order_number,created_at,created_by) VALUES
    (1,1,1,1,CURRENT_TIMESTAMP,'init')
  , (1,1,2,2,CURRENT_TIMESTAMP,'init');

-- 検査メニュー特記_コード
insert into exam_menu_note_codes(code,name,created_at,created_by) VALUES
    ('ST01','撮影番号',CURRENT_TIMESTAMP,'init')
  , ('ST02','○○番号',CURRENT_TIMESTAMP,'init');

-- 検査メニュー特記_受診
insert into exam_menu_note_consults(menu_note_id,code,order_number,created_at,created_by) VALUES
    (2,'ST01',1,CURRENT_TIMESTAMP,'init')
  , (2,'ST02',2,CURRENT_TIMESTAMP,'init');

-- 受診特記
insert into consult_notes(consult_id, code, note, created_at, created_by) VALUES
    ('8dda2a54-5217-425f-bba9-ab821a9647fe','ST01','01-001',CURRENT_TIMESTAMP,'init')
  , ('8dda2a54-5217-425f-bba9-ab821a9647fe','ST02','02-001',CURRENT_TIMESTAMP,'init');

-- 基準値パターン
insert into thresholds(threshold_id, threshold_code, name, order_number, created_at, created_by) VALUES
    ('aa114e6d-c63e-4f44-96fa-42989129147a','701','収縮期血圧',1,CURRENT_TIMESTAMP,'init')
  , ('a71b27d3-4cd6-46ce-a99c-d998574029f1','702','拡張期血圧',2,CURRENT_TIMESTAMP,'init');

-- 基準値
insert into consult_thresholds(threshold_id, consult_id, priority, created_at, created_by) VALUES
    ('aa114e6d-c63e-4f44-96fa-42989129147a','8dda2a54-5217-425f-bba9-ab821a9647fe',1,CURRENT_TIMESTAMP,'init')
  , ('a71b27d3-4cd6-46ce-a99c-d998574029f1','8dda2a54-5217-425f-bba9-ab821a9647fe',1,CURRENT_TIMESTAMP,'init');

-- 検査基準値範囲
insert into exam_normal_value_range(name, threshold_id, exam_item_detail_id, min_age, max_age, target_sex, min_value, max_value, error_level, created_at, created_by) VALUES
    ('収縮期（50以上300以下）正常範囲',  'aa114e6d-c63e-4f44-96fa-42989129147a',711,'00000','9999999',3,50,300,1,CURRENT_TIMESTAMP,'init')
    , ('拡張期（10以上200以下）正常範囲','a71b27d3-4cd6-46ce-a99c-d998574029f1',712,'00000','9999999',3,10,200,1,CURRENT_TIMESTAMP,'init')
    , ('収縮期（50以上300以下）正常範囲','aa114e6d-c63e-4f44-96fa-42989129147a',721,'00000','9999999',3,50,300,1,CURRENT_TIMESTAMP,'init')
    , ('拡張期（10以上200以下）正常範囲','a71b27d3-4cd6-46ce-a99c-d998574029f1',722,'00000','9999999',3,10,200,1,CURRENT_TIMESTAMP,'init')
    , ('収縮期（50未満）下限警告',       'aa114e6d-c63e-4f44-96fa-42989129147a',711,'00000','9999999',3,0,49,2,CURRENT_TIMESTAMP,'init')
    , ('拡張期（10未満）下限警告',       'a71b27d3-4cd6-46ce-a99c-d998574029f1',712,'00000','9999999',3,0,9,2,CURRENT_TIMESTAMP,'init')
    , ('収縮期（50未満）下限警告',       'aa114e6d-c63e-4f44-96fa-42989129147a',721,'00000','9999999',3,0,49,2,CURRENT_TIMESTAMP,'init')
    , ('拡張期（10未満）下限警告',       'a71b27d3-4cd6-46ce-a99c-d998574029f1',722,'00000','9999999',3,0,9,2,CURRENT_TIMESTAMP,'init')              
    , ('収縮期（300以上）上限異常',      'aa114e6d-c63e-4f44-96fa-42989129147a',711,'00000','9999999',3,301,9999,3,CURRENT_TIMESTAMP,'init')
    , ('拡張期（200以上）上限異常',      'a71b27d3-4cd6-46ce-a99c-d998574029f1',712,'00000','9999999',3,201,9999,3,CURRENT_TIMESTAMP,'init')
    , ('収縮期（300以上）上限異常',      'aa114e6d-c63e-4f44-96fa-42989129147a',721,'00000','9999999',3,301,9999,3,CURRENT_TIMESTAMP,'init')
    , ('拡張期（200以上）上限異常',      'a71b27d3-4cd6-46ce-a99c-d998574029f1',722,'00000','9999999',3,201,9999,3,CURRENT_TIMESTAMP,'init');

INSERT INTO resultcollector.thresholds(threshold_id,threshold_code,name,order_number,created_by) VALUES 
    ('36853b96-be4b-419b-a9c4-01ab0714eef3','common','共通',2,'init')
  , ('d5ae9e47-c9b3-410a-8963-669966daf9fc','ryobi','両備ドック',1,'init');

INSERT INTO resultcollector.exam_normal_value_range(range_id,name,threshold_id,exam_item_detail_id,min_age,max_age,target_sex,min_value,max_value,error_level,created_by) VALUES 
    ('012c2d8a-82cb-487c-a45a-dc78de90c0c7','共通_血圧_下_警告2','36853b96-be4b-419b-a9c4-01ab0714eef3',712,'0000000','9999999',3,90,999,2,'init')
  , ('159a5871-c6d1-4b5f-9637-6faae4983633','両備ドック_男_血圧_下_警告1','d5ae9e47-c9b3-410a-8963-669966daf9fc',712,'0000000','9999999',1,0.0,50,2,'init')
  , ('3915b419-3314-48ab-88e0-d54b2c44efcc','両備ドック_女_血圧_下_警告2','d5ae9e47-c9b3-410a-8963-669966daf9fc',712,'0000000','9999999',2,90,999,2,'init')
  , ('406766b6-9c83-48bb-9819-b95a4a7e7742','両備ドック_男_血圧_下_正常域','d5ae9e47-c9b3-410a-8963-669966daf9fc',712,'0000000','9999999',1,50,90,1,'init')
  , ('4bed6a27-c84a-44a1-bd29-81c9e5d72c11','両備ドック_男_血圧_上_警告2','d5ae9e47-c9b3-410a-8963-669966daf9fc',711,'0000000','9999999',1,140,999,2,'init')
  , ('4c4d2161-259d-4821-b394-f6412721eec4','両備ドック_女_血圧_下_正常域','d5ae9e47-c9b3-410a-8963-669966daf9fc',712,'0000000','9999999',2,50,90,1,'init')
  , ('4e1a276e-2c95-437a-81d0-d4af7076f086','共通_血圧_上_警告2','36853b96-be4b-419b-a9c4-01ab0714eef3',711,'0000000','9999999',3,140,999,2,'init')
  , ('67cf19b9-6425-4b7d-abb0-fe7bc9ea0af1','共通_血圧_下_警告1','36853b96-be4b-419b-a9c4-01ab0714eef3',712,'0000000','9999999',3,0.0,50,2,'init')
  , ('77cc6d96-4113-449b-b6d0-10c9331a3ddc','両備ドック_女_血圧_上_警告1','d5ae9e47-c9b3-410a-8963-669966daf9fc',711,'0000000','9999999',2,0.0,100,2,'init')
  , ('911042ed-08b9-42d5-af12-5157849dcdb5','共通_血圧_下_正常域','36853b96-be4b-419b-a9c4-01ab0714eef3',712,'0000000','9999999',3,50,90,1,'init')
  , ('98af16cd-6a5d-4add-85a6-4767dd4b50fe','両備ドック_男_血圧_上_警告1','d5ae9e47-c9b3-410a-8963-669966daf9fc',711,'0000000','9999999',1,0.0,100,2,'init')
  , ('aa8a2f66-dd5d-4807-93b2-9e841b0f990c','両備ドック_男_血圧_下_警告2','d5ae9e47-c9b3-410a-8963-669966daf9fc',712,'0000000','9999999',1,90,999,2,'init')
  , ('c950cc2a-ceb6-482a-8635-3482d39eed18','両備ドック_女_血圧_上_正常域','d5ae9e47-c9b3-410a-8963-669966daf9fc',711,'0000000','9999999',2,100,140,1,'init')
  , ('d45c70c3-9714-4f51-a7b7-f749173b0eea','両備ドック_男_血圧_上_正常域','d5ae9e47-c9b3-410a-8963-669966daf9fc',711,'0000000','9999999',1,100,140,1,'init')
  , ('dce76a0c-3136-45b7-b2a7-2a6fe4117b99','両備ドック_女_血圧_下_警告1','d5ae9e47-c9b3-410a-8963-669966daf9fc',712,'0000000','9999999',2,0.0,50,2,'init')
  , ('f34a642f-ba7b-44c0-95d1-8011e904ce7b','共通_血圧_上_警告1','36853b96-be4b-419b-a9c4-01ab0714eef3',711,'0000000','9999999',3,0.0,100,2,'init')
  , ('f85a50df-5c43-448d-bbbf-c8dfc026ff65','両備ドック_女_血圧_上_警告2','d5ae9e47-c9b3-410a-8963-669966daf9fc',711,'0000000','9999999',2,140,999,2,'init')
  , ('fcc036b7-28b5-402d-b42b-df060fb733af','共通_血圧_上_正常域','36853b96-be4b-419b-a9c4-01ab0714eef3',711,'0000000','9999999',3,100,140,1,'init');

INSERT INTO resultcollector.consult_thresholds(threshold_id,consult_id,priority,created_by) VALUES 
    ('d5ae9e47-c9b3-410a-8963-669966daf9fc','caaaaa00-0000-0000-0000-000000000001',1,'init')
  , ('36853b96-be4b-419b-a9c4-01ab0714eef3','caaaaa00-0000-0000-0000-000000000001',2,'init');
-- 検査実施判定ルール
insert into decision_rules(decision_rule_id,name,exam_menu_id,priority,trigger_type,error_level,message,created_at,created_by) VALUES 
    (1,'体重差が前年より30kgオーバー',7,1,1,3,'体重の計測ミスのため実施できません。',CURRENT_TIMESTAMP,'init');

-- 検査実施判断ルール_判定値
insert into decision_rule_evaluations(decision_rule_id,variable_number,evaluation_value,created_at,created_by) VALUES 
    (1,1,30.0,CURRENT_TIMESTAMP,'init');

-- 検査実施判断ルール_検査項目明細
insert into decision_rule_exam_item_details(decision_rule_id,variable_number,source_type,exam_item_detail_id,created_at,created_by) VALUES
    (1,1,1,2,CURRENT_TIMESTAMP,'init')
  , (1,2,2,2,CURRENT_TIMESTAMP,'init');

-- アプリケーション設定
INSERT INTO app_config(key,value,description,created_at,created_by) VALUES 
    ('Auth.AccessTokenLifetime','3','アクセストークンの有効期限（分）',CURRENT_TIMESTAMP,'init')
  , ('Auth.RefreshTokenLifeTime','720','リフレッシュトークンの有効期限（分）',CURRENT_TIMESTAMP,'init')
  , ('Auth.SecretKey','qguSdqCvPiGwUceBYXWEfJKLNrakzsnblVjxRIApFhmQtOHoZDTy','トークンのシークレット',CURRENT_TIMESTAMP,'init');