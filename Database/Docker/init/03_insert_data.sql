-- DB切り替え
\c dev01

set search_path = resultcollector;

-- 受診者
INSERT INTO examinees(examinee_id,examinee_code,name,kana_name,sex,birthdate,created_at,created_by) VALUES 
    (1,'100001','両備　花子001','リョウビ　ハナコ001',2,DATE '2012-11-19',CURRENT_TIMESTAMP,'init')
  , (2,'100002','両備　花子002','リョウビ　ハナコ002',2,DATE '1939-03-16',CURRENT_TIMESTAMP,'init')
  , (3,'100003','両備　花子003','リョウビ　ハナコ003',2,DATE '2001-04-29',CURRENT_TIMESTAMP,'init')
  , (4,'100004','両備　花子004','リョウビ　ハナコ004',2,DATE '1986-02-02',CURRENT_TIMESTAMP,'init')
  , (5,'100005','両備　花子005','リョウビ　ハナコ005',2,DATE '1962-09-21',CURRENT_TIMESTAMP,'init')
  , (6,'100006','両備　花子006','リョウビ　ハナコ006',2,DATE '1942-05-16',CURRENT_TIMESTAMP,'init')
  , (7,'100007','両備　花子007','リョウビ　ハナコ007',2,DATE '1976-02-11',CURRENT_TIMESTAMP,'init')
  , (8,'100008','両備　花子008','リョウビ　ハナコ008',2,DATE '2006-05-20',CURRENT_TIMESTAMP,'init')
  , (9,'100009','両備　花子009','リョウビ　ハナコ009',2,DATE '1976-07-01',CURRENT_TIMESTAMP,'init')
  , (10,'100010','両備　花子010','リョウビ　ハナコ010',2,DATE '1952-01-21',CURRENT_TIMESTAMP,'init')
  , (11,'100011','両備　太郎011','リョウビ　タロウ011',1,DATE '1957-05-25',CURRENT_TIMESTAMP,'init')
  , (12,'100012','両備　太郎012','リョウビ　タロウ012',1,DATE '2024-11-12',CURRENT_TIMESTAMP,'init')
  , (13,'100013','両備　太郎013','リョウビ　タロウ013',1,DATE '2002-05-11',CURRENT_TIMESTAMP,'init')
  , (14,'100014','両備　太郎014','リョウビ　タロウ014',1,DATE '1991-06-07',CURRENT_TIMESTAMP,'init')
  , (15,'100015','両備　太郎015','リョウビ　タロウ015',1,DATE '1999-01-05',CURRENT_TIMESTAMP,'init')
  , (16,'100016','両備　太郎016','リョウビ　タロウ016',1,DATE '1999-05-15',CURRENT_TIMESTAMP,'init')
  , (17,'100017','両備　太郎017','リョウビ　タロウ017',1,DATE '1924-04-01',CURRENT_TIMESTAMP,'init')
  , (18,'100018','両備　太郎018','リョウビ　タロウ018',1,DATE '1957-06-16',CURRENT_TIMESTAMP,'init')
  , (19,'100019','両備　太郎019','リョウビ　タロウ019',1,DATE '1982-09-18',CURRENT_TIMESTAMP,'init')
  , (20,'100020','両備　太郎020','リョウビ　タロウ020',1,DATE '1926-03-25',CURRENT_TIMESTAMP,'init');

-- 会場
INSERT INTO places(place_id,place_code,name,order_number,created_at,created_by) VALUES 
    (1,'P001','会場A',1,CURRENT_TIMESTAMP,'init')
  , (2,'P002','会場B',2,CURRENT_TIMESTAMP,'init')
  , (3,'P003','会場C',3,CURRENT_TIMESTAMP,'init')
  , (4,'P004','会場D',4,CURRENT_TIMESTAMP,'init')
  , (5,'P005','会場E',5,CURRENT_TIMESTAMP,'init');

-- 班
INSERT INTO teams(team_id,team_code,name,order_number,created_at,created_by) VALUES 
    (1,'T001','班A',1,CURRENT_TIMESTAMP,'init')
  , (2,'T002','班B',2,CURRENT_TIMESTAMP,'init')
  , (3,'T003','班C',3,CURRENT_TIMESTAMP,'init');

-- 会場日程
INSERT INTO place_schedule(place_schedule_id,place_id,team_id,status,exam_date,start_time,created_at,created_by) VALUES 
    (1,1,1,21,DATE '2024-10-01','0900',CURRENT_TIMESTAMP,'init')
  , (2,1,1,21,DATE '2024-10-01','1300',CURRENT_TIMESTAMP,'init')
  , (3,1,2,21,DATE '2024-11-02','1000',CURRENT_TIMESTAMP,'init');

-- 検査メニュー
INSERT INTO exam_menus(exam_menu_id,name,order_number,created_at,created_by) VALUES 
    (1,'身体計測',1,CURRENT_TIMESTAMP,'init')
  , (2,'視力',2,CURRENT_TIMESTAMP,'init')
  , (3,'聴力',3,CURRENT_TIMESTAMP,'init')
  , (4,'心電図',4,CURRENT_TIMESTAMP,'init')
  , (5,'腹囲',5,CURRENT_TIMESTAMP,'init')
  , (6,'眼底',6,CURRENT_TIMESTAMP,'init')
  , (7,'血圧',7,CURRENT_TIMESTAMP,'init')
  , (8,'眼圧',8,CURRENT_TIMESTAMP,'init')
  , (9,'握力',9,CURRENT_TIMESTAMP,'init');

-- 検査項目グループ
INSERT INTO exam_item_groups(exam_item_group_id,name,exam_menu_id,type,created_at,created_by) VALUES 
    (1,'身体計測',1,1,CURRENT_TIMESTAMP,'init')
  , (7,'血圧',7,7,CURRENT_TIMESTAMP,'init');

-- 検査項目
INSERT INTO exam_items(exam_item_id,name,exam_item_group_id,position_number,unit,created_at,created_by) VALUES 
    (1,'身長',1,1,'cm',CURRENT_TIMESTAMP,'init')
  , (2,'体重',1,1,'kg',CURRENT_TIMESTAMP,'init')
  , (71,'血圧1',7,1,null,CURRENT_TIMESTAMP,'init')
  , (72,'血圧2',7,2,null,CURRENT_TIMESTAMP,'init');

-- 検査項目明細
INSERT INTO exam_item_details(exam_item_detail_id,exam_item_id,set_previous_as_default,position_number,type,keyboard_type,created_at,created_by) VALUES 
    (1,1,False,1,1,0,CURRENT_TIMESTAMP,'init')
  , (2,2,False,1,1,0,CURRENT_TIMESTAMP,'init')
  , (711,71,False,1,1,0,CURRENT_TIMESTAMP,'init')
  , (712,71,False,2,1,0,CURRENT_TIMESTAMP,'init')
  , (721,72,False,1,1,0,CURRENT_TIMESTAMP,'init')
  , (722,72,False,2,1,0,CURRENT_TIMESTAMP,'init');


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
INSERT INTO consult(consult_id,consult_number,progress_status,export_status,place_schedule_id,note,examinee_id,external_connection_code,created_at,created_by) VALUES 
    (1,'0001',11,11,2,'',2,'10001',CURRENT_TIMESTAMP,'init')
  , (2,'0002',41,11,2,'',12,'10002',CURRENT_TIMESTAMP,'init')
  , (3,'0003',51,11,1,'',18,'10003',CURRENT_TIMESTAMP,'init')
  , (4,'0004',41,21,2,'',13,'10004',CURRENT_TIMESTAMP,'init')
  , (5,'0005',41,31,1,'',14,'10005',CURRENT_TIMESTAMP,'init');

-- ロール
INSERT INTO roles(role_id,name,created_at,created_by) VALUES 
    (10,'一般',CURRENT_TIMESTAMP,'init')
  , (20,'管理者',CURRENT_TIMESTAMP,'init');

-- 職員
INSERT INTO staffs(staff_id,staff_code,login_id,name,password_hash,password_salt,enabled,role_id,created_at,created_by) VALUES 
    (1,'S001','S001','職員A',DECODE('cndlYnJ0MzU2Nzh0ZXNuZXNydHNlbXRhM3c0YW0zNGF3NG1hdzM0bWE0cWEzNG1yNzg=', 'base64'),DECODE('d3JhT1VZSVJUVDRFNXJld3J0', 'base64'),True,10,CURRENT_TIMESTAMP,'init')
  , (2,'S002','S002','職員B',DECODE('bWh0anV1ZHI1dXlzZXJ5', 'base64'),DECODE('ZXdiNWFxMzRyYmdlbnN5cmVtc2VyeQ==', 'base64'),True,20,CURRENT_TIMESTAMP,'init');

-- 団体
INSERT INTO organizations(organization_id,organization_code,name,order_number,created_at,created_by) VALUES 
    (1,'11','株式会社テストA',1,CURRENT_TIMESTAMP,'init')
  , (2,'12','株式会社テストB',2,CURRENT_TIMESTAMP,'init')
  , (3,'13','株式会社テストC',3,CURRENT_TIMESTAMP,'init')
  , (4,'21','テストD株式会社',4,CURRENT_TIMESTAMP,'init')
  , (5,'22','テストE株式会社',5,CURRENT_TIMESTAMP,'init')
  , (6,'23','一般社団法人テストF',6,CURRENT_TIMESTAMP,'init');

-- 検査機器
INSERT INTO equipments(equipment_id,name,exam_menu_id,app_launch_url,processing_script_url,created_at,created_by) VALUES 
    (1,'EQ001',1,'wsc://abcde','https://example.com/sctipts/eq001.js',CURRENT_TIMESTAMP,'init')
  , (2,'EQ002',1,'wsc://abcde','https://example.com/sctipts/eq002.js',CURRENT_TIMESTAMP,'init')
  , (3,'EQ003',2,'wsc://abcde','https://example.com/sctipts/eq003.js',CURRENT_TIMESTAMP,'init')
  , (4,'EQ004',3,'wsc://abcde','https://example.com/sctipts/eq004.js',CURRENT_TIMESTAMP,'init')
  , (5,'EQ005',5,'wsc://abcde','https://example.com/sctipts/eq005.js',CURRENT_TIMESTAMP,'init');

-- 検査結果出力履歴
INSERT INTO export_histories(id, place_schedule_id,exported_at,exported_by,created_at,created_by) VALUES 
    ('af0971a0-186f-4b14-86a1-0c648d77c9af',2,TIMESTAMP '2024-11-25 12:57:15.506','職員B',CURRENT_TIMESTAMP,'init')
  , ('41293e22-4290-4f85-9179-9affda055cdd',3,TIMESTAMP '2024-10-22 20:00:00.000','職員A',CURRENT_TIMESTAMP,'init')
  , ('b53cc211-1b6a-4d20-8bb0-3c2a007661c6',1,TIMESTAMP '2024-10-03 13:20:00.000','職員B',CURRENT_TIMESTAMP,'init')
  , ('8f2edd5a-9ad3-4445-96a8-1b2129f60929',1,TIMESTAMP '2023-02-25 09:24:000.000','職員A',CURRENT_TIMESTAMP,'init');

-- 検査結果出力履歴明細
INSERT INTO export_history_details(id,consult_id,created_at,created_by) VALUES 
    ('af0971a0-186f-4b14-86a1-0c648d77c9af',1,CURRENT_TIMESTAMP,'init')
  , ('af0971a0-186f-4b14-86a1-0c648d77c9af',2,CURRENT_TIMESTAMP,'init')
  , ('af0971a0-186f-4b14-86a1-0c648d77c9af',4,CURRENT_TIMESTAMP,'init')
  , ('b53cc211-1b6a-4d20-8bb0-3c2a007661c6',3,CURRENT_TIMESTAMP,'init')
  , ('8f2edd5a-9ad3-4445-96a8-1b2129f60929',5,CURRENT_TIMESTAMP,'init');

-- 検査項目明細依頼
INSERT INTO exam_item_detail_orders(consult_id,exam_item_detail_id,created_at,created_by) VALUES 
    (1,1,CURRENT_TIMESTAMP,'init')
  , (1,2,CURRENT_TIMESTAMP,'init')
  , (1,711,CURRENT_TIMESTAMP,'init')
  , (1,712,CURRENT_TIMESTAMP,'init')
  , (1,721,CURRENT_TIMESTAMP,'init')
  , (1,722,CURRENT_TIMESTAMP,'init')
  , (2,1,CURRENT_TIMESTAMP,'init')
  , (2,2,CURRENT_TIMESTAMP,'init')
  , (2,711,CURRENT_TIMESTAMP,'init')
  , (2,712,CURRENT_TIMESTAMP,'init')
  , (2,721,CURRENT_TIMESTAMP,'init')
  , (2,722,CURRENT_TIMESTAMP,'init')
  , (3,1,CURRENT_TIMESTAMP,'init')
  , (3,2,CURRENT_TIMESTAMP,'init')
  , (3,711,CURRENT_TIMESTAMP,'init')
  , (3,712,CURRENT_TIMESTAMP,'init')
  , (4,1,CURRENT_TIMESTAMP,'init')
  , (4,2,CURRENT_TIMESTAMP,'init')
  , (4,711,CURRENT_TIMESTAMP,'init')
  , (4,712,CURRENT_TIMESTAMP,'init')
  , (4,721,CURRENT_TIMESTAMP,'init')
  , (4,722,CURRENT_TIMESTAMP,'init');

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
    (1,1,'178',CURRENT_TIMESTAMP,'init')
  , (1,2,'70',CURRENT_TIMESTAMP,'init')
  , (1,711,'72',CURRENT_TIMESTAMP,'init')
  , (1,712,'98',CURRENT_TIMESTAMP,'init')
  , (1,721,'70',CURRENT_TIMESTAMP,'init')
  , (1,722,'100',CURRENT_TIMESTAMP,'init')
  , (2,1,'160',CURRENT_TIMESTAMP,'init')
  , (2,711,'72',CURRENT_TIMESTAMP,'init')
  , (2,721,'70',CURRENT_TIMESTAMP,'init')
  , (2,722,'100',CURRENT_TIMESTAMP,'init')
  , (3,1,'180',CURRENT_TIMESTAMP,'init')
  , (3,2,'90',CURRENT_TIMESTAMP,'init')
  , (3,711,'92',CURRENT_TIMESTAMP,'init')
  , (3,712,'128',CURRENT_TIMESTAMP,'init')
  , (4,711,'92',CURRENT_TIMESTAMP,'init')
  , (4,712,'128',CURRENT_TIMESTAMP,'init');

-- 検査中止
INSERT INTO exam_cancels(consult_id,exam_item_detail_id,cancel_reason_id,created_at,created_by) VALUES 
    (4,1,1,CURRENT_TIMESTAMP,'init')
  , (4,2,2,CURRENT_TIMESTAMP,'init')
  , (4,721,10,CURRENT_TIMESTAMP,'init');
