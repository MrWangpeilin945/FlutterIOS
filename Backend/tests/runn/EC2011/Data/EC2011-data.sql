insert into resultcollector.staffs(staff_code, login_id, name, password_hash, password_salt, enabled, role_id, created_at, created_by)
    values('201103','S201103', '一般職員_甲', '00', '00' , true, 10,  CURRENT_TIMESTAMP, 'ex2011')
    ,     ('201104','S201104', '一般職員_乙', '00', '00' , false, 10,  CURRENT_TIMESTAMP, 'ex2011')
    ,     ('201105','S201105', '既存ログインID', '00', '00' , true, 10,  CURRENT_TIMESTAMP, 'ex2011')
    ,     ('201106','S201106', '既存職員ID', '00', '00' , true, 10,  CURRENT_TIMESTAMP, 'ex2011');
