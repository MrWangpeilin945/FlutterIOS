import { atom } from "jotai";
import { atomWithStorage, RESET } from "jotai/utils";
import type {
  StringIdNamedEntity,
  NumberIdNamedEntity,
  ConnectionEquipment,
} from "~/interfaces/interfaces";
import type { PlaceSchedule } from "~/domain/wellship.schemas";

// 状態管理はすべてlocalStorageに保存する(atomWithStorage)
export const teamState = atomWithStorage<StringIdNamedEntity | null>(
  "teamState",
  null,
);
export const placeScheduleState = atomWithStorage<PlaceSchedule | null>(
  "placeScheduleState",
  null,
);
export const examDateState = atomWithStorage<Date | null>(
  "examDateState",
  null,
);
export const staffState = atomWithStorage<StringIdNamedEntity | null>(
  "staffState",
  null,
);
export const examMenuState = atomWithStorage<NumberIdNamedEntity[] | null>(
  "examMenuState",
  null,
);
export const connectionEquipmentState = atomWithStorage<
  ConnectionEquipment[] | null
>("connectionEquipmentState", null);
export const serverTimeOffsetState = atomWithStorage<number | null>(
  "serverTimeOffsetState",
  null,
);

// 状態管理をクリアする(ログアウト処理で使用)
export const clearAllState = atom(null, (_get, set) => {
  set(teamState, RESET);
  set(placeScheduleState, RESET);
  set(examDateState, RESET);
  set(staffState, RESET);
  set(examMenuState, RESET);
  set(connectionEquipmentState, RESET);
  set(serverTimeOffsetState, RESET);
});

// 状態管理をクリアする(班選択画面で使用)
export const clearExamState = atom(null,(_get, set) =>{
  set(placeScheduleState, RESET);
  set(examDateState, RESET);
  set(examMenuState, RESET);
})
