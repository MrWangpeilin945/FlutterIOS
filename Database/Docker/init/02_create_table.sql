-- DB切り替え
\c sampledb

set search_path = sampleschema;

-- 班
create table teams (
  team_id integer not null
  , name text
  , constraint teams_PKC primary key (team_id)
) ;

-- 会場
create table places (
  place_id integer not null
  , name text
  , constraint places_PKC primary key (place_id)
) ;

-- 会場日程
create table place_schedules (
  place_schedule_id integer not null
  , team_id integer not null
  , place_id integer not null
  , end_time time with time zone not null
  , start_time time with time zone not null
  , date date not null
  , constraint place_schedules_PKC primary key (place_schedule_id)
) ;

comment on table place_schedules is '会場日程:会場と日程の組み合わせで健診実施単位。';
comment on column place_schedules.place_schedule_id is '会場日程ID';
comment on column place_schedules.team_id is '班ID';
comment on column place_schedules.place_id is '会場ID';
comment on column place_schedules.end_time is '終了時刻';
comment on column place_schedules.start_time is '開始時刻';
comment on column place_schedules.date is '実施日';

comment on table teams is '班';
comment on column teams.team_id is '班ID';
comment on column teams.name is '班名';

comment on table places is '会場';
comment on column places.place_id is '会場ID';
comment on column places.name is '会場名';


-- 権限追加
GRANT ALL PRIVILEGES ON place_schedules TO sample;
GRANT ALL PRIVILEGES ON teams TO sample;
GRANT ALL PRIVILEGES ON places TO sample;

-- teams テーブルのダミーデータの挿入
INSERT INTO teams (team_id, name)
VALUES
  (1, '1班'),
  (2, '2班'),
  (3, '3班');

-- places テーブルのダミーデータの挿入
INSERT INTO places (place_id, name)
VALUES
  (10, '両備システムズ豊成オフィス'),
  (20, '両備システムズ藤崎オフィス');

-- place_schedules テーブルのダミーデータの挿入
INSERT INTO place_schedules (place_schedule_id, team_id, place_id, end_time, start_time, date)
VALUES
  (1, 2, 10, '09:00:00', '12:00:00', '2024-09-27'),
  (2, 2, 10, '09:00:00', '12:00:00', '2024-12-01'),
  (3, 3, 10, '09:00:00', '12:00:00', '2024-09-27'),
  (4, 2, 20, '13:00:00', '17:00:00', '2024-09-27');
