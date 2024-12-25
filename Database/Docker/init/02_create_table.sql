-- DB切り替え
\c dev01

set search_path = resultcollector;

CREATE TABLE affiliations (
  examinee_id uuid NOT NULL
  , organization_id uuid NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT affiliations_PKC PRIMARY KEY (examinee_id,organization_id)
);

CREATE TABLE consult_notes (
  consult_id uuid NOT NULL
  , code text NOT NULL
  , note text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT consult_notes_PKC PRIMARY KEY (consult_id,code)
);

CREATE TABLE consult_thresholds (
  threshold_id uuid NOT NULL
  , consult_id uuid NOT NULL
  , priority integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT consult_thresholds_PKC PRIMARY KEY (threshold_id,consult_id)
);

CREATE TABLE correlation_rule_evaluations (
  correlation_rule_id integer NOT NULL
  , variable_number integer NOT NULL
  , evaluation_value text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT correlation_rule_evaluations_PKC PRIMARY KEY (correlation_rule_id,variable_number)
);

CREATE TABLE correlation_rule_exam_item_details (
  correlation_rule_id integer NOT NULL
  , variable_number integer NOT NULL
  , source_type integer NOT NULL
  , exam_item_detail_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT correlation_rule_exam_item_details_PKC PRIMARY KEY (correlation_rule_id,variable_number)
);

CREATE TABLE correlation_rules (
  correlation_rule_id integer NOT NULL
  , name text NOT NULL
  , exam_menu_id integer NOT NULL
  , priority integer NOT NULL
  , trigger_type integer NOT NULL
  , error_level integer NOT NULL
  , exam_item_id integer NOT NULL
  , message text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT correlation_rules_PKC PRIMARY KEY (correlation_rule_id)
);

ALTER TABLE correlation_rules ADD CONSTRAINT correlation_rules_IX1
  UNIQUE (exam_menu_id,priority) ;

CREATE TABLE decision_rule_evaluations (
  decision_rule_id integer NOT NULL
  , variable_number integer NOT NULL
  , evaluation_value text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT decision_rule_evaluations_PKC PRIMARY KEY (decision_rule_id,variable_number)
);

CREATE TABLE decision_rule_exam_item_details (
  decision_rule_id integer NOT NULL
  , variable_number integer NOT NULL
  , source_type integer NOT NULL
  , exam_item_detail_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT decision_rule_exam_item_details_PKC PRIMARY KEY (decision_rule_id,variable_number)
);

CREATE TABLE decision_rules (
  decision_rule_id integer NOT NULL
  , name text NOT NULL
  , exam_menu_id integer NOT NULL
  , priority integer NOT NULL
  , trigger_type integer NOT NULL
  , error_level integer NOT NULL
  , message text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT decision_rules_PKC PRIMARY KEY (decision_rule_id)
);

CREATE TABLE equipments (
  equipment_id integer NOT NULL
  , name text NOT NULL
  , exam_menu_id integer NOT NULL
  , app_launch_url text NOT NULL
  , processing_script_url text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT equipments_PKC PRIMARY KEY (equipment_id)
);

CREATE TABLE exam_cancel_histories (
  id uuid DEFAULT gen_random_uuid () NOT NULL
  , consult_id uuid NOT NULL
  , consult_item_detail_id integer NOT NULL
  , value text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_cancel_histories_PKC PRIMARY KEY (id)
);

CREATE TABLE exam_cancels (
  consult_id uuid NOT NULL
  , exam_item_detail_id integer NOT NULL
  , cancel_reason_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_cancels_PKC PRIMARY KEY (consult_id,exam_item_detail_id)
);

CREATE TABLE exam_item_detail_orders (
  consult_id uuid NOT NULL
  , exam_item_detail_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_item_detail_orders_PKC PRIMARY KEY (consult_id,exam_item_detail_id)
);

CREATE TABLE exam_menu_note_consults (
  menu_note_id integer NOT NULL
  , code text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_menu_note_consults_PKC PRIMARY KEY (menu_note_id,code)
);

CREATE TABLE exam_menu_note_details (
  code text NOT NULL
  , name text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_menu_note_details_PKC PRIMARY KEY (code)
);

CREATE TABLE exam_menu_note_results (
  menu_note_id integer NOT NULL
  , exam_item_detail_id integer NOT NULL
  , source_type integer NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_menu_note_results_PKC PRIMARY KEY (menu_note_id,exam_item_detail_id,source_type)
);

CREATE TABLE exam_menu_notes (
  menu_note_id integer NOT NULL
  , name text NOT NULL
  , exam_menu_id integer NOT NULL
  , order_number integer NOT NULL
  , suffix text
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_menu_notes_PKC PRIMARY KEY (menu_note_id)
);

ALTER TABLE exam_menu_notes ADD CONSTRAINT exam_menu_notes_IX1
  UNIQUE (exam_menu_id,order_number) ;

CREATE TABLE exam_normal_option_details (
  normal_options_id uuid NOT NULL
  , option_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_normal_option_details_PKC PRIMARY KEY (normal_options_id,option_id)
);

CREATE TABLE exam_normal_options (
  normal_options_id uuid DEFAULT gen_random_uuid () NOT NULL
  , name text NOT NULL
  , threshold_id uuid NOT NULL
  , exam_item_detail_id integer NOT NULL
  , max_age varchar(7) NOT NULL
  , min_age varchar(7) NOT NULL
  , target_sex integer NOT NULL
  , error_level integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_normal_options_PKC PRIMARY KEY (normal_options_id)
);

ALTER TABLE exam_normal_options ADD CONSTRAINT exam_normal_options_IX1
  UNIQUE (threshold_id,exam_item_detail_id,max_age,target_sex,error_level) ;

CREATE TABLE exam_normal_value_range (
  range_id uuid DEFAULT gen_random_uuid () NOT NULL
  , name text NOT NULL
  , threshold_id uuid NOT NULL
  , exam_item_detail_id integer NOT NULL
  , max_age varchar(7) NOT NULL
  , min_age varchar(7) NOT NULL
  , target_sex integer NOT NULL
  , max_value decimal NOT NULL
  , min_value decimal NOT NULL
  , error_level integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_normal_value_range_PKC PRIMARY KEY (range_id)
);

ALTER TABLE exam_normal_value_range ADD CONSTRAINT exam_normal_value_range_IX1
  UNIQUE (threshold_id,exam_item_detail_id,max_age,target_sex,max_value) ;

CREATE TABLE exam_result_histories (
  id uuid DEFAULT gen_random_uuid () NOT NULL
  , consult_id uuid NOT NULL
  , exam_item_detail_id integer NOT NULL
  , value text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_result_histories_PKC PRIMARY KEY (id)
);

CREATE TABLE exam_results (
  consult_id uuid NOT NULL
  , exam_item_detail_id integer NOT NULL
  , value text
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_results_PKC PRIMARY KEY (consult_id,exam_item_detail_id)
);

CREATE TABLE export_history_details (
  id uuid NOT NULL
  , consult_id uuid NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT export_history_details_PKC PRIMARY KEY (id,consult_id)
);

CREATE TABLE external_exam_item_details (
  exam_item_detail_id integer NOT NULL
  , external_exam_item_detail_code text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT external_exam_item_details_PKC PRIMARY KEY (exam_item_detail_id,external_exam_item_detail_code)
);

CREATE TABLE home_menus (
  home_menu_id integer NOT NULL
  , name text NOT NULL
  , home_menu_group_id integer NOT NULL
  , order_number integer NOT NULL
  , path text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT home_menus_PKC PRIMARY KEY (home_menu_id)
);

ALTER TABLE home_menus ADD CONSTRAINT home_menus_IX1
  UNIQUE (path) ;

CREATE TABLE keyboard_options (
  option_id integer NOT NULL
  , exam_item_detail_id integer NOT NULL
  , value text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT keyboard_options_PKC PRIMARY KEY (option_id)
);

CREATE TABLE organizations (
  organization_id uuid DEFAULT gen_random_uuid () NOT NULL
  , organization_code text NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT organizations_PKC PRIMARY KEY (organization_id)
);

ALTER TABLE organizations ADD CONSTRAINT organizations_IX1
  UNIQUE (organization_code) ;

CREATE TABLE place_schedule_lock_histoies (
  id uuid DEFAULT gen_random_uuid () NOT NULL
  , place_schedule_id uuid NOT NULL
  , status integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT place_schedule_lock_histoies_PKC PRIMARY KEY (id)
);

CREATE TABLE previous_results (
  consult_id uuid NOT NULL
  , exam_date date NOT NULL
  , exam_item_detail_id integer NOT NULL
  , value text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT previous_results_PKC PRIMARY KEY (consult_id,exam_date,exam_item_detail_id)
);

CREATE TABLE prior_exam_menus (
  current_exam_menu_id integer NOT NULL
  , prior_exam_menu_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT prior_exam_menus_PKC PRIMARY KEY (current_exam_menu_id,prior_exam_menu_id)
);

CREATE TABLE staff_login_histories (
  id uuid DEFAULT gen_random_uuid () NOT NULL
  , staff_id uuid NOT NULL
  , login_timestamp timestamp with time zone NOT NULL
  , login_success boolean NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT staff_login_histories_PKC PRIMARY KEY (id)
);

CREATE TABLE staffs (
  staff_id uuid DEFAULT gen_random_uuid () NOT NULL
  , staff_code text NOT NULL
  , login_id text NOT NULL
  , name text NOT NULL
  , password_hash bytea NOT NULL
  , password_salt bytea NOT NULL
  , enabled boolean DEFAULT true NOT NULL
  , role_id integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT staffs_PKC PRIMARY KEY (staff_id)
);

ALTER TABLE staffs ADD CONSTRAINT staffs_IX1
  UNIQUE (staff_code) ;

ALTER TABLE staffs ADD CONSTRAINT staffs_IX2
  UNIQUE (login_id) ;

CREATE TABLE thresholds (
  threshold_id uuid DEFAULT gen_random_uuid () NOT NULL
  , threshold_code text NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT thresholds_PKC PRIMARY KEY (threshold_id)
);

CREATE UNIQUE INDEX thresholds_IX1
  ON thresholds(threshold_code);

CREATE TABLE tickets (
  consult_id uuid NOT NULL
  , ticket_number text
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT tickets_PKC PRIMARY KEY (consult_id)
);

CREATE TABLE tickets_histories (
  id uuid NOT NULL
  , consult_id uuid NOT NULL
  , ticket_number text
  , action_type varchar(1) NOT NULL
  , order integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT tickets_histories_PKC PRIMARY KEY (id)
);

CREATE TABLE cancel_reasons (
  cancel_reason_id integer NOT NULL
  , name text NOT NULL
  , exam_item_id integer NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT cancel_reasons_PKC PRIMARY KEY (cancel_reason_id)
);

CREATE TABLE consult (
  consult_id uuid DEFAULT gen_random_uuid () NOT NULL
  , consult_number text NOT NULL
  , progress_status integer NOT NULL
  , export_status integer NOT NULL
  , place_schedule_id uuid NOT NULL
  , note text NOT NULL
  , examinee_id uuid NOT NULL
  , external_connection_code text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT consult_PKC PRIMARY KEY (consult_id)
);

ALTER TABLE consult ADD CONSTRAINT consult_IX1
  UNIQUE (consult_number) ;

CREATE UNIQUE INDEX consult_IX2
  ON consult(external_connection_code);

CREATE TABLE exam_item_detail_options (
  option_id integer NOT NULL
  , code text NOT NULL
  , exam_item_detail_id integer NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_item_detail_options_PKC PRIMARY KEY (option_id)
);

CREATE TABLE exam_item_details (
  exam_item_detail_id integer NOT NULL
  , exam_item_id integer NOT NULL
  , name text NOT NULL
  , set_previous_as_default boolean DEFAULT false NOT NULL
  , order_number integer NOT NULL
  , position_number integer NOT NULL
  , type integer NOT NULL
  , keyboard_type integer NOT NULL
  , integer_length integer NOT NULL
  , decimal_length integer NOT NULL
  , equipment_label text
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_item_details_PKC PRIMARY KEY (exam_item_detail_id)
);

CREATE TABLE exam_items (
  exam_item_id integer NOT NULL
  , name text NOT NULL
  , exam_item_group_id integer NOT NULL
  , position_number integer NOT NULL
  , unit text
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_items_PKC PRIMARY KEY (exam_item_id)
);

CREATE TABLE examinees (
  examinee_id uuid DEFAULT gen_random_uuid () NOT NULL
  , examinee_code text NOT NULL
  , name text NOT NULL
  , kana_name text NOT NULL
  , sex integer NOT NULL
  , birthdate date NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT examinees_PKC PRIMARY KEY (examinee_id)
);

ALTER TABLE examinees ADD CONSTRAINT examinees_IX1
  UNIQUE (examinee_code) ;

CREATE TABLE export_histories (
  id uuid DEFAULT gen_random_uuid () NOT NULL
  , place_schedule_id uuid NOT NULL
  , exported_at timestamp with time zone NOT NULL
  , exported_by text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT export_histories_PKC PRIMARY KEY (id)
);

CREATE TABLE home_menu_groups (
  home_menu_group_id integer NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT home_menu_groups_PKC PRIMARY KEY (home_menu_group_id)
);

CREATE TABLE place_schedule (
  place_schedule_id uuid DEFAULT gen_random_uuid () NOT NULL
  , place_id uuid NOT NULL
  , team_id uuid NOT NULL
  , status integer NOT NULL
  , exam_date date NOT NULL
  , start_time varchar(4) NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT place_schedule_PKC PRIMARY KEY (place_schedule_id)
);

ALTER TABLE place_schedule ADD CONSTRAINT place_schedule_IX1
  UNIQUE (place_id,team_id,exam_date) ;

CREATE TABLE places (
  place_id uuid DEFAULT gen_random_uuid () NOT NULL
  , place_code text NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT places_PKC PRIMARY KEY (place_id)
);

ALTER TABLE places ADD CONSTRAINT places_IX1
  UNIQUE (place_code) ;

CREATE TABLE roles (
  role_id integer NOT NULL
  , name text NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT roles_PKC PRIMARY KEY (role_id)
);

CREATE TABLE teams (
  team_id uuid DEFAULT gen_random_uuid () NOT NULL
  , team_code text NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT teams_PKC PRIMARY KEY (team_id)
);

ALTER TABLE teams ADD CONSTRAINT teams_IX1
  UNIQUE (team_code) ;

CREATE TABLE exam_item_groups (
  exam_item_group_id integer NOT NULL
  , name text NOT NULL
  , exam_menu_id integer NOT NULL
  , type integer NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_item_groups_PKC PRIMARY KEY (exam_item_group_id)
);

CREATE TABLE exam_menus (
  exam_menu_id integer NOT NULL
  , name text NOT NULL
  , order_number integer NOT NULL
  , created_at timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
  , created_by text NOT NULL
  , CONSTRAINT exam_menus_PKC PRIMARY KEY (exam_menu_id)
);

ALTER TABLE affiliations
  ADD CONSTRAINT affiliations_FK1 FOREIGN KEY (examinee_id) REFERENCES examinees(examinee_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE affiliations
  ADD CONSTRAINT affiliations_FK2 FOREIGN KEY (organization_id) REFERENCES organizations(organization_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE cancel_reasons
  ADD CONSTRAINT cancel_reasons_FK1 FOREIGN KEY (exam_item_id) REFERENCES exam_items(exam_item_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE consult
  ADD CONSTRAINT consult_FK1 FOREIGN KEY (examinee_id) REFERENCES examinees(examinee_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE consult
  ADD CONSTRAINT consult_FK2 FOREIGN KEY (place_schedule_id) REFERENCES place_schedule(place_schedule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE consult_notes
  ADD CONSTRAINT consult_notes_FK1 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE consult_thresholds
  ADD CONSTRAINT consult_thresholds_FK1 FOREIGN KEY (threshold_id) REFERENCES thresholds(threshold_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE consult_thresholds
  ADD CONSTRAINT consult_thresholds_FK2 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE correlation_rule_evaluations
  ADD CONSTRAINT correlation_rule_evaluations_FK1 FOREIGN KEY (correlation_rule_id) REFERENCES correlation_rules(correlation_rule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE correlation_rule_exam_item_details
  ADD CONSTRAINT correlation_rule_exam_item_details_FK1 FOREIGN KEY (correlation_rule_id) REFERENCES correlation_rules(correlation_rule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE correlation_rule_exam_item_details
  ADD CONSTRAINT correlation_rule_exam_item_details_FK2 FOREIGN KEY (exam_item_detail_id) REFERENCES exam_item_details(exam_item_detail_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE correlation_rules
  ADD CONSTRAINT correlation_rules_FK1 FOREIGN KEY (exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE decision_rule_evaluations
  ADD CONSTRAINT decision_rule_evaluations_FK1 FOREIGN KEY (decision_rule_id) REFERENCES decision_rules(decision_rule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE decision_rule_exam_item_details
  ADD CONSTRAINT decision_rule_exam_item_details_FK1 FOREIGN KEY (decision_rule_id) REFERENCES decision_rules(decision_rule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE decision_rule_exam_item_details
  ADD CONSTRAINT decision_rule_exam_item_details_FK2 FOREIGN KEY (exam_item_detail_id) REFERENCES exam_item_details(exam_item_detail_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE decision_rules
  ADD CONSTRAINT decision_rules_FK1 FOREIGN KEY (exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE equipments
  ADD CONSTRAINT equipments_FK1 FOREIGN KEY (exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_cancels
  ADD CONSTRAINT exam_cancels_FK1 FOREIGN KEY (cancel_reason_id) REFERENCES cancel_reasons(cancel_reason_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_cancels
  ADD CONSTRAINT exam_cancels_FK2 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_item_detail_options
  ADD CONSTRAINT exam_item_detail_options_FK1 FOREIGN KEY (exam_item_detail_id) REFERENCES exam_item_details(exam_item_detail_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_item_detail_orders
  ADD CONSTRAINT exam_item_detail_orders_FK1 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_item_details
  ADD CONSTRAINT exam_item_details_FK1 FOREIGN KEY (exam_item_id) REFERENCES exam_items(exam_item_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_item_groups
  ADD CONSTRAINT exam_item_groups_FK1 FOREIGN KEY (exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_items
  ADD CONSTRAINT exam_items_FK1 FOREIGN KEY (exam_item_group_id) REFERENCES exam_item_groups(exam_item_group_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_menu_note_consults
  ADD CONSTRAINT exam_menu_note_consults_FK1 FOREIGN KEY (menu_note_id) REFERENCES exam_menu_notes(menu_note_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_menu_note_consults
  ADD CONSTRAINT exam_menu_note_consults_FK2 FOREIGN KEY (code) REFERENCES exam_menu_note_details(code)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_menu_note_results
  ADD CONSTRAINT exam_menu_note_results_FK1 FOREIGN KEY (menu_note_id) REFERENCES exam_menu_notes(menu_note_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_menu_note_results
  ADD CONSTRAINT exam_menu_note_results_FK2 FOREIGN KEY (exam_item_detail_id) REFERENCES exam_item_details(exam_item_detail_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_menu_notes
  ADD CONSTRAINT exam_menu_notes_FK1 FOREIGN KEY (exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_normal_option_details
  ADD CONSTRAINT exam_normal_option_details_FK1 FOREIGN KEY (normal_options_id) REFERENCES exam_normal_options(normal_options_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_normal_option_details
  ADD CONSTRAINT exam_normal_option_details_FK2 FOREIGN KEY (option_id) REFERENCES exam_item_detail_options(option_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE exam_results
  ADD CONSTRAINT exam_results_FK1 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE export_histories
  ADD CONSTRAINT export_histories_FK1 FOREIGN KEY (place_schedule_id) REFERENCES place_schedule(place_schedule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE export_history_details
  ADD CONSTRAINT export_history_details_FK1 FOREIGN KEY (id) REFERENCES export_histories(id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE home_menus
  ADD CONSTRAINT home_menus_FK1 FOREIGN KEY (home_menu_group_id) REFERENCES home_menu_groups(home_menu_group_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE keyboard_options
  ADD CONSTRAINT keyboard_options_FK1 FOREIGN KEY (exam_item_detail_id) REFERENCES exam_item_details(exam_item_detail_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE place_schedule
  ADD CONSTRAINT place_schedule_FK1 FOREIGN KEY (place_id) REFERENCES places(place_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE place_schedule
  ADD CONSTRAINT place_schedule_FK2 FOREIGN KEY (team_id) REFERENCES teams(team_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE place_schedule_lock_histoies
  ADD CONSTRAINT place_schedule_lock_histoies_FK1 FOREIGN KEY (place_schedule_id) REFERENCES place_schedule(place_schedule_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE previous_results
  ADD CONSTRAINT previous_results_FK1 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE prior_exam_menus
  ADD CONSTRAINT prior_exam_menus_FK1 FOREIGN KEY (current_exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE prior_exam_menus
  ADD CONSTRAINT prior_exam_menus_FK2 FOREIGN KEY (prior_exam_menu_id) REFERENCES exam_menus(exam_menu_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE staff_login_histories
  ADD CONSTRAINT staff_login_histories_FK1 FOREIGN KEY (staff_id) REFERENCES staffs(staff_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE staffs
  ADD CONSTRAINT staffs_FK1 FOREIGN KEY (role_id) REFERENCES roles(role_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE tickets
  ADD CONSTRAINT tickets_FK1 FOREIGN KEY (consult_id) REFERENCES consult(consult_id)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

COMMENT ON TABLE affiliations IS '所属';
COMMENT ON COLUMN affiliations.examinee_id IS '受診者ID';
COMMENT ON COLUMN affiliations.organization_id IS '団体ID';
COMMENT ON COLUMN affiliations.created_at IS '作成日時';
COMMENT ON COLUMN affiliations.created_by IS '作成者';

COMMENT ON TABLE consult_notes IS '受診特記';
COMMENT ON COLUMN consult_notes.consult_id IS '受診ID';
COMMENT ON COLUMN consult_notes.code IS '検査特記コード';
COMMENT ON COLUMN consult_notes.note IS '特記事項';
COMMENT ON COLUMN consult_notes.created_at IS '作成日時';
COMMENT ON COLUMN consult_notes.created_by IS '作成者';

COMMENT ON TABLE consult_thresholds IS '基準値';
COMMENT ON COLUMN consult_thresholds.threshold_id IS '基準値パターンID';
COMMENT ON COLUMN consult_thresholds.consult_id IS '受診ID';
COMMENT ON COLUMN consult_thresholds.priority IS '優先度';
COMMENT ON COLUMN consult_thresholds.created_at IS '作成日時';
COMMENT ON COLUMN consult_thresholds.created_by IS '作成者';

COMMENT ON TABLE correlation_rule_evaluations IS '検査結果相関ルール_判定値';
COMMENT ON COLUMN correlation_rule_evaluations.correlation_rule_id IS '検査結果相関ルールID';
COMMENT ON COLUMN correlation_rule_evaluations.variable_number IS '変数番号';
COMMENT ON COLUMN correlation_rule_evaluations.evaluation_value IS '判定値';
COMMENT ON COLUMN correlation_rule_evaluations.created_at IS '作成日時';
COMMENT ON COLUMN correlation_rule_evaluations.created_by IS '作成者';

COMMENT ON TABLE correlation_rule_exam_item_details IS '検査結果相関ルール_検査項目明細';
COMMENT ON COLUMN correlation_rule_exam_item_details.correlation_rule_id IS '検査結果相関ルール';
COMMENT ON COLUMN correlation_rule_exam_item_details.variable_number IS '変数番号';
COMMENT ON COLUMN correlation_rule_exam_item_details.source_type IS 'データソース種別';
COMMENT ON COLUMN correlation_rule_exam_item_details.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN correlation_rule_exam_item_details.created_at IS '作成日時';
COMMENT ON COLUMN correlation_rule_exam_item_details.created_by IS '作成者';

COMMENT ON TABLE correlation_rules IS '検査結果相関ルール';
COMMENT ON COLUMN correlation_rules.correlation_rule_id IS '検査結果相関ルールID';
COMMENT ON COLUMN correlation_rules.name IS '名称';
COMMENT ON COLUMN correlation_rules.exam_menu_id IS '検査メニューID';
COMMENT ON COLUMN correlation_rules.priority IS '優先度';
COMMENT ON COLUMN correlation_rules.trigger_type IS '発火条件種別';
COMMENT ON COLUMN correlation_rules.error_level IS 'エラーレベル';
COMMENT ON COLUMN correlation_rules.exam_item_id IS '検査項目ID_出力用';
COMMENT ON COLUMN correlation_rules.message IS '出力メッセージ';
COMMENT ON COLUMN correlation_rules.created_at IS '作成日時';
COMMENT ON COLUMN correlation_rules.created_by IS '作成者';

COMMENT ON TABLE decision_rule_evaluations IS '検査実施判断ルール_判定値';
COMMENT ON COLUMN decision_rule_evaluations.decision_rule_id IS '検査実施判断ルールID';
COMMENT ON COLUMN decision_rule_evaluations.variable_number IS '変数番号';
COMMENT ON COLUMN decision_rule_evaluations.evaluation_value IS '判定値';
COMMENT ON COLUMN decision_rule_evaluations.created_at IS '作成日時';
COMMENT ON COLUMN decision_rule_evaluations.created_by IS '作成者';

COMMENT ON TABLE decision_rule_exam_item_details IS '検査実施判断ルール_検査項目明細';
COMMENT ON COLUMN decision_rule_exam_item_details.decision_rule_id IS '検査実施判断ルール';
COMMENT ON COLUMN decision_rule_exam_item_details.variable_number IS '変数番号';
COMMENT ON COLUMN decision_rule_exam_item_details.source_type IS 'データソース種別';
COMMENT ON COLUMN decision_rule_exam_item_details.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN decision_rule_exam_item_details.created_at IS '作成日時';
COMMENT ON COLUMN decision_rule_exam_item_details.created_by IS '作成者';

COMMENT ON TABLE decision_rules IS '検査実施判断ルール';
COMMENT ON COLUMN decision_rules.decision_rule_id IS '検査実施判断ルールID';
COMMENT ON COLUMN decision_rules.name IS '名称';
COMMENT ON COLUMN decision_rules.exam_menu_id IS '検査メニューID';
COMMENT ON COLUMN decision_rules.priority IS '優先度';
COMMENT ON COLUMN decision_rules.trigger_type IS '発火条件種別';
COMMENT ON COLUMN decision_rules.error_level IS 'エラーレベル';
COMMENT ON COLUMN decision_rules.message IS '出力メッセージ';
COMMENT ON COLUMN decision_rules.created_at IS '作成日時';
COMMENT ON COLUMN decision_rules.created_by IS '作成者';

COMMENT ON TABLE equipments IS '検査機器';
COMMENT ON COLUMN equipments.equipment_id IS '検査機器ID';
COMMENT ON COLUMN equipments.name IS '検査機器名';
COMMENT ON COLUMN equipments.exam_menu_id IS '検査メニューID';
COMMENT ON COLUMN equipments.app_launch_url IS 'アプリ起動URL';
COMMENT ON COLUMN equipments.processing_script_url IS '処理スクリプトURL';
COMMENT ON COLUMN equipments.created_at IS '作成日時';
COMMENT ON COLUMN equipments.created_by IS '作成者';

COMMENT ON TABLE exam_cancel_histories IS '検査中止履歴';
COMMENT ON COLUMN exam_cancel_histories.id IS 'ID';
COMMENT ON COLUMN exam_cancel_histories.consult_id IS '受診ID';
COMMENT ON COLUMN exam_cancel_histories.consult_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_cancel_histories.value IS '値';
COMMENT ON COLUMN exam_cancel_histories.created_at IS '作成日時';
COMMENT ON COLUMN exam_cancel_histories.created_by IS '作成者';

COMMENT ON TABLE exam_cancels IS '検査中止';
COMMENT ON COLUMN exam_cancels.consult_id IS '受診ID';
COMMENT ON COLUMN exam_cancels.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_cancels.cancel_reason_id IS '中止理由';
COMMENT ON COLUMN exam_cancels.created_at IS '作成日時';
COMMENT ON COLUMN exam_cancels.created_by IS '作成者';

COMMENT ON TABLE exam_item_detail_orders IS '検査項目明細依頼';
COMMENT ON COLUMN exam_item_detail_orders.consult_id IS '受診ID';
COMMENT ON COLUMN exam_item_detail_orders.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_item_detail_orders.created_at IS '作成日時';
COMMENT ON COLUMN exam_item_detail_orders.created_by IS '作成者';

COMMENT ON TABLE exam_menu_note_consults IS '検査メニュー特記_受診';
COMMENT ON COLUMN exam_menu_note_consults.menu_note_id IS '検査メニュー特記ID';
COMMENT ON COLUMN exam_menu_note_consults.code IS '検査特記コード';
COMMENT ON COLUMN exam_menu_note_consults.order_number IS '表示順';
COMMENT ON COLUMN exam_menu_note_consults.created_at IS '作成日時';
COMMENT ON COLUMN exam_menu_note_consults.created_by IS '作成者';

COMMENT ON TABLE exam_menu_note_details IS '検査特記明細';
COMMENT ON COLUMN exam_menu_note_details.code IS '検査特記コード';
COMMENT ON COLUMN exam_menu_note_details.name IS '検査特記名';
COMMENT ON COLUMN exam_menu_note_details.created_at IS '作成日時';
COMMENT ON COLUMN exam_menu_note_details.created_by IS '作成者';

COMMENT ON TABLE exam_menu_note_results IS '検査メニュー特記_検査結果';
COMMENT ON COLUMN exam_menu_note_results.menu_note_id IS '検査メニュー特記ID';
COMMENT ON COLUMN exam_menu_note_results.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_menu_note_results.source_type IS 'データソース種別';
COMMENT ON COLUMN exam_menu_note_results.order_number IS '表示順';
COMMENT ON COLUMN exam_menu_note_results.created_at IS '作成日時';
COMMENT ON COLUMN exam_menu_note_results.created_by IS '作成者';

COMMENT ON TABLE exam_menu_notes IS '検査メニュー特記';
COMMENT ON COLUMN exam_menu_notes.menu_note_id IS '検査メニュー特記ID';
COMMENT ON COLUMN exam_menu_notes.name IS '名称';
COMMENT ON COLUMN exam_menu_notes.exam_menu_id IS '検査メニューID';
COMMENT ON COLUMN exam_menu_notes.order_number IS '表示順';
COMMENT ON COLUMN exam_menu_notes.suffix IS '接尾辞';
COMMENT ON COLUMN exam_menu_notes.created_at IS '作成日時';
COMMENT ON COLUMN exam_menu_notes.created_by IS '作成者';

COMMENT ON TABLE exam_normal_option_details IS '検査項目明細_選択肢';
COMMENT ON COLUMN exam_normal_option_details.normal_options_id IS '基準値選択肢ID';
COMMENT ON COLUMN exam_normal_option_details.option_id IS '選択肢ID';
COMMENT ON COLUMN exam_normal_option_details.created_at IS '作成日時';
COMMENT ON COLUMN exam_normal_option_details.created_by IS '作成者';

COMMENT ON TABLE exam_normal_options IS '検査基準値選択肢';
COMMENT ON COLUMN exam_normal_options.normal_options_id IS '基準値選択肢ID';
COMMENT ON COLUMN exam_normal_options.name IS '名称';
COMMENT ON COLUMN exam_normal_options.threshold_id IS '基準値パターンID:0: テナントの基準';
COMMENT ON COLUMN exam_normal_options.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_normal_options.max_age IS '対象年齢上限';
COMMENT ON COLUMN exam_normal_options.min_age IS '対象年齢下限';
COMMENT ON COLUMN exam_normal_options.target_sex IS '対象性別';
COMMENT ON COLUMN exam_normal_options.error_level IS 'エラーレベル';
COMMENT ON COLUMN exam_normal_options.created_at IS '作成日時';
COMMENT ON COLUMN exam_normal_options.created_by IS '作成者';

COMMENT ON TABLE exam_normal_value_range IS '検査基準値範囲';
COMMENT ON COLUMN exam_normal_value_range.range_id IS '範囲ID';
COMMENT ON COLUMN exam_normal_value_range.name IS '名称';
COMMENT ON COLUMN exam_normal_value_range.threshold_id IS '基準値パターンID:0: テナントの基準';
COMMENT ON COLUMN exam_normal_value_range.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_normal_value_range.max_age IS '対象年齢上限';
COMMENT ON COLUMN exam_normal_value_range.min_age IS '対象年齢下限';
COMMENT ON COLUMN exam_normal_value_range.target_sex IS '対象性別';
COMMENT ON COLUMN exam_normal_value_range.max_value IS '値上限';
COMMENT ON COLUMN exam_normal_value_range.min_value IS '値下限';
COMMENT ON COLUMN exam_normal_value_range.error_level IS 'エラーレベル';
COMMENT ON COLUMN exam_normal_value_range.created_at IS '作成日時';
COMMENT ON COLUMN exam_normal_value_range.created_by IS '作成者';

COMMENT ON TABLE exam_result_histories IS '検査結果履歴';
COMMENT ON COLUMN exam_result_histories.id IS 'ID';
COMMENT ON COLUMN exam_result_histories.consult_id IS '受診ID';
COMMENT ON COLUMN exam_result_histories.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_result_histories.value IS '値';
COMMENT ON COLUMN exam_result_histories.created_at IS '作成日時';
COMMENT ON COLUMN exam_result_histories.created_by IS '作成者';

COMMENT ON TABLE exam_results IS '検査結果';
COMMENT ON COLUMN exam_results.consult_id IS '受診ID';
COMMENT ON COLUMN exam_results.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_results.value IS '値';
COMMENT ON COLUMN exam_results.created_at IS '作成日時';
COMMENT ON COLUMN exam_results.created_by IS '作成者';

COMMENT ON TABLE export_history_details IS '検査結果出力履歴明細';
COMMENT ON COLUMN export_history_details.id IS 'ID';
COMMENT ON COLUMN export_history_details.consult_id IS '受診ID';
COMMENT ON COLUMN export_history_details.created_at IS '作成日時';
COMMENT ON COLUMN export_history_details.created_by IS '作成者';

COMMENT ON TABLE external_exam_item_details IS '外部検査項目明細';
COMMENT ON COLUMN external_exam_item_details.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN external_exam_item_details.external_exam_item_detail_code IS '外部コード検査項目明細CD';
COMMENT ON COLUMN external_exam_item_details.created_at IS '作成日時';
COMMENT ON COLUMN external_exam_item_details.created_by IS '作成者';

COMMENT ON TABLE home_menus IS 'ホームメニュー';
COMMENT ON COLUMN home_menus.home_menu_id IS 'ホームメニューID';
COMMENT ON COLUMN home_menus.name IS 'ホームメニュー名';
COMMENT ON COLUMN home_menus.home_menu_group_id IS 'ホームメニューグループID';
COMMENT ON COLUMN home_menus.order_number IS 'グループ内表示順';
COMMENT ON COLUMN home_menus.path IS 'パス';
COMMENT ON COLUMN home_menus.created_at IS '作成日時';
COMMENT ON COLUMN home_menus.created_by IS '作成者';

COMMENT ON TABLE keyboard_options IS 'キーボード入力値リスト';
COMMENT ON COLUMN keyboard_options.option_id IS 'ID';
COMMENT ON COLUMN keyboard_options.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN keyboard_options.value IS '入力値';
COMMENT ON COLUMN keyboard_options.created_at IS '作成日時';
COMMENT ON COLUMN keyboard_options.created_by IS '作成者';

COMMENT ON TABLE organizations IS '団体';
COMMENT ON COLUMN organizations.organization_id IS '団体ID';
COMMENT ON COLUMN organizations.organization_code IS '団体コード';
COMMENT ON COLUMN organizations.name IS '団体名';
COMMENT ON COLUMN organizations.order_number IS '表示順';
COMMENT ON COLUMN organizations.created_at IS '作成日時';
COMMENT ON COLUMN organizations.created_by IS '作成者';

COMMENT ON TABLE place_schedule_lock_histoies IS '会場ロック履歴';
COMMENT ON COLUMN place_schedule_lock_histoies.id IS 'ID';
COMMENT ON COLUMN place_schedule_lock_histoies.place_schedule_id IS '会場日程ID';
COMMENT ON COLUMN place_schedule_lock_histoies.status IS '状況';
COMMENT ON COLUMN place_schedule_lock_histoies.created_at IS '作成日時';
COMMENT ON COLUMN place_schedule_lock_histoies.created_by IS '作成者';

COMMENT ON TABLE previous_results IS '過去検査結果';
COMMENT ON COLUMN previous_results.consult_id IS '受診ID';
COMMENT ON COLUMN previous_results.exam_date IS '健診日';
COMMENT ON COLUMN previous_results.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN previous_results.value IS '値';
COMMENT ON COLUMN previous_results.created_at IS '作成日時';
COMMENT ON COLUMN previous_results.created_by IS '作成者';

COMMENT ON TABLE prior_exam_menus IS '前提検査メニュー';
COMMENT ON COLUMN prior_exam_menus.current_exam_menu_id IS '現在検査メニューID';
COMMENT ON COLUMN prior_exam_menus.prior_exam_menu_id IS '前提検査メニューID';
COMMENT ON COLUMN prior_exam_menus.created_at IS '作成日時';
COMMENT ON COLUMN prior_exam_menus.created_by IS '作成者';

COMMENT ON TABLE staff_login_histories IS '職員ログイン履歴';
COMMENT ON COLUMN staff_login_histories.id IS 'ID';
COMMENT ON COLUMN staff_login_histories.staff_id IS '職員ID';
COMMENT ON COLUMN staff_login_histories.login_timestamp IS 'ログイン試行日時';
COMMENT ON COLUMN staff_login_histories.login_success IS 'ログイン成功フラグ';
COMMENT ON COLUMN staff_login_histories.created_at IS '作成日時';
COMMENT ON COLUMN staff_login_histories.created_by IS '作成者';

COMMENT ON TABLE staffs IS '職員';
COMMENT ON COLUMN staffs.staff_id IS '職員ID';
COMMENT ON COLUMN staffs.staff_code IS '職員コード';
COMMENT ON COLUMN staffs.login_id IS 'ログインID';
COMMENT ON COLUMN staffs.name IS '職員名';
COMMENT ON COLUMN staffs.password_hash IS 'パスワードハッシュ';
COMMENT ON COLUMN staffs.password_salt IS 'パスワードソルト';
COMMENT ON COLUMN staffs.enabled IS '有効';
COMMENT ON COLUMN staffs.role_id IS 'ロールID';
COMMENT ON COLUMN staffs.created_at IS '作成日時';
COMMENT ON COLUMN staffs.created_by IS '作成者';

COMMENT ON TABLE thresholds IS '基準値パターン';
COMMENT ON COLUMN thresholds.threshold_id IS '基準値パターンID';
COMMENT ON COLUMN thresholds.threshold_code IS '基準値パターンコード';
COMMENT ON COLUMN thresholds.name IS '基準値パターン名';
COMMENT ON COLUMN thresholds.order_number IS '表示順';
COMMENT ON COLUMN thresholds.created_at IS '作成日時';
COMMENT ON COLUMN thresholds.created_by IS '作成者';

COMMENT ON TABLE tickets IS '受付';
COMMENT ON COLUMN tickets.consult_id IS '受診ID';
COMMENT ON COLUMN tickets.ticket_number IS '受付番号';
COMMENT ON COLUMN tickets.created_at IS '作成日時';
COMMENT ON COLUMN tickets.created_by IS '作成者';

COMMENT ON TABLE tickets_histories IS '受付履歴';
COMMENT ON COLUMN tickets_histories.id IS 'ID';
COMMENT ON COLUMN tickets_histories.consult_id IS '受診ID';
COMMENT ON COLUMN tickets_histories.ticket_number IS '受付番号';
COMMENT ON COLUMN tickets_histories.action_type IS '操作区分:I:Ins/U:Upd/D:Del';
COMMENT ON COLUMN tickets_histories.order IS '登録順:一括処理時';
COMMENT ON COLUMN tickets_histories.created_at IS '作成日時';
COMMENT ON COLUMN tickets_histories.created_by IS '作成者';

COMMENT ON TABLE cancel_reasons IS '中止理由';
COMMENT ON COLUMN cancel_reasons.cancel_reason_id IS '中止理由ID';
COMMENT ON COLUMN cancel_reasons.name IS '中止理由名';
COMMENT ON COLUMN cancel_reasons.exam_item_id IS '検査項目ID';
COMMENT ON COLUMN cancel_reasons.order_number IS '表示順';
COMMENT ON COLUMN cancel_reasons.created_at IS '作成日時';
COMMENT ON COLUMN cancel_reasons.created_by IS '作成者';

COMMENT ON TABLE consult IS '受診';
COMMENT ON COLUMN consult.consult_id IS '受診ID';
COMMENT ON COLUMN consult.consult_number IS '受診番号';
COMMENT ON COLUMN consult.progress_status IS '進捗状況';
COMMENT ON COLUMN consult.export_status IS '結果出力状況';
COMMENT ON COLUMN consult.place_schedule_id IS '会場日程ID';
COMMENT ON COLUMN consult.note IS '特記事項';
COMMENT ON COLUMN consult.examinee_id IS '受診者ID';
COMMENT ON COLUMN consult.external_connection_code IS '外部連携キー';
COMMENT ON COLUMN consult.created_at IS '作成日時';
COMMENT ON COLUMN consult.created_by IS '作成者';

COMMENT ON TABLE exam_item_detail_options IS '検査項目明細_選択肢';
COMMENT ON COLUMN exam_item_detail_options.option_id IS '選択肢ID';
COMMENT ON COLUMN exam_item_detail_options.code IS 'コード';
COMMENT ON COLUMN exam_item_detail_options.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_item_detail_options.name IS '名称';
COMMENT ON COLUMN exam_item_detail_options.order_number IS '表示順';
COMMENT ON COLUMN exam_item_detail_options.created_at IS '作成日時';
COMMENT ON COLUMN exam_item_detail_options.created_by IS '作成者';

COMMENT ON TABLE exam_item_details IS '検査項目明細';
COMMENT ON COLUMN exam_item_details.exam_item_detail_id IS '検査項目明細ID';
COMMENT ON COLUMN exam_item_details.exam_item_id IS '検査項目ID';
COMMENT ON COLUMN exam_item_details.name IS '検査項目明細名';
COMMENT ON COLUMN exam_item_details.set_previous_as_default IS '前回値を初期値としてセットするか';
COMMENT ON COLUMN exam_item_details.order_number IS '表示順';
COMMENT ON COLUMN exam_item_details.position_number IS '配置番号';
COMMENT ON COLUMN exam_item_details.type IS '検査項目明細種別';
COMMENT ON COLUMN exam_item_details.keyboard_type IS 'キーボード種別';
COMMENT ON COLUMN exam_item_details.integer_length IS '整数部桁数';
COMMENT ON COLUMN exam_item_details.decimal_length IS '小数部桁数';
COMMENT ON COLUMN exam_item_details.equipment_label IS '機器ラベル';
COMMENT ON COLUMN exam_item_details.created_at IS '作成日時';
COMMENT ON COLUMN exam_item_details.created_by IS '作成者';

COMMENT ON TABLE exam_items IS '検査項目';
COMMENT ON COLUMN exam_items.exam_item_id IS '検査項目ID';
COMMENT ON COLUMN exam_items.name IS '検査項目名';
COMMENT ON COLUMN exam_items.exam_item_group_id IS '検査項目グループID';
COMMENT ON COLUMN exam_items.position_number IS '配置番号';
COMMENT ON COLUMN exam_items.unit IS '単位';
COMMENT ON COLUMN exam_items.order_number IS '表示順';
COMMENT ON COLUMN exam_items.created_at IS '作成日時';
COMMENT ON COLUMN exam_items.created_by IS '作成者';

COMMENT ON TABLE examinees IS '受診者';
COMMENT ON COLUMN examinees.examinee_id IS '受診者ID';
COMMENT ON COLUMN examinees.examinee_code IS '受診者コード';
COMMENT ON COLUMN examinees.name IS '氏名';
COMMENT ON COLUMN examinees.kana_name IS 'カナ氏名';
COMMENT ON COLUMN examinees.sex IS '性別';
COMMENT ON COLUMN examinees.birthdate IS '生年月日';
COMMENT ON COLUMN examinees.created_at IS '作成日時';
COMMENT ON COLUMN examinees.created_by IS '作成者';

COMMENT ON TABLE export_histories IS '検査結果出力履歴';
COMMENT ON COLUMN export_histories.id IS 'ID';
COMMENT ON COLUMN export_histories.place_schedule_id IS '会場日程ID';
COMMENT ON COLUMN export_histories.exported_at IS '出力日時';
COMMENT ON COLUMN export_histories.exported_by IS '出力者';
COMMENT ON COLUMN export_histories.created_at IS '作成日時';
COMMENT ON COLUMN export_histories.created_by IS '作成者';

COMMENT ON TABLE home_menu_groups IS 'ホームメニューグループ';
COMMENT ON COLUMN home_menu_groups.home_menu_group_id IS 'ホームメニューグループID';
COMMENT ON COLUMN home_menu_groups.name IS 'ホームメニューグループ名';
COMMENT ON COLUMN home_menu_groups.order_number IS '表示順';
COMMENT ON COLUMN home_menu_groups.created_at IS '作成日時';
COMMENT ON COLUMN home_menu_groups.created_by IS '作成者';

COMMENT ON TABLE place_schedule IS '会場日程';
COMMENT ON COLUMN place_schedule.place_schedule_id IS '会場日程ID';
COMMENT ON COLUMN place_schedule.place_id IS '会場ID';
COMMENT ON COLUMN place_schedule.team_id IS '班ID';
COMMENT ON COLUMN place_schedule.status IS '状況';
COMMENT ON COLUMN place_schedule.exam_date IS '健診日';
COMMENT ON COLUMN place_schedule.start_time IS '開始時刻';
COMMENT ON COLUMN place_schedule.created_at IS '作成日時';
COMMENT ON COLUMN place_schedule.created_by IS '作成者';

COMMENT ON TABLE places IS '会場';
COMMENT ON COLUMN places.place_id IS '会場ID';
COMMENT ON COLUMN places.place_code IS '会場コード';
COMMENT ON COLUMN places.name IS '会場名';
COMMENT ON COLUMN places.order_number IS '表示順';
COMMENT ON COLUMN places.created_at IS '作成日時';
COMMENT ON COLUMN places.created_by IS '作成者';

COMMENT ON TABLE roles IS 'ロール';
COMMENT ON COLUMN roles.role_id IS 'ロールID';
COMMENT ON COLUMN roles.name IS 'ロール名';
COMMENT ON COLUMN roles.created_at IS '作成日時';
COMMENT ON COLUMN roles.created_by IS '作成者';

COMMENT ON TABLE teams IS '班';
COMMENT ON COLUMN teams.team_id IS '班ID';
COMMENT ON COLUMN teams.team_code IS '班コード';
COMMENT ON COLUMN teams.name IS '班名';
COMMENT ON COLUMN teams.order_number IS '表示順';
COMMENT ON COLUMN teams.created_at IS '作成日時';
COMMENT ON COLUMN teams.created_by IS '作成者';

COMMENT ON TABLE exam_item_groups IS '検査項目グループ';
COMMENT ON COLUMN exam_item_groups.exam_item_group_id IS '検査項目グループID';
COMMENT ON COLUMN exam_item_groups.name IS '検査項目グループ名';
COMMENT ON COLUMN exam_item_groups.exam_menu_id IS '検査メニューID';
COMMENT ON COLUMN exam_item_groups.type IS '検査項目グループ種別';
COMMENT ON COLUMN exam_item_groups.order_number IS '表示順';
COMMENT ON COLUMN exam_item_groups.created_at IS '作成日時';
COMMENT ON COLUMN exam_item_groups.created_by IS '作成者';

COMMENT ON TABLE exam_menus IS '検査メニュー';
COMMENT ON COLUMN exam_menus.exam_menu_id IS '検査メニューID';
COMMENT ON COLUMN exam_menus.name IS '検査メニュー名';
COMMENT ON COLUMN exam_menus.order_number IS '表示順';
COMMENT ON COLUMN exam_menus.created_at IS '作成日時';
COMMENT ON COLUMN exam_menus.created_by IS '作成者';
