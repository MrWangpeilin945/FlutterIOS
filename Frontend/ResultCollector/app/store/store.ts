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
  undefined,
  { getOnInit: true },
);
export const placeScheduleState = atomWithStorage<PlaceSchedule | null>(
  "placeScheduleState",
  null,
  undefined,
  { getOnInit: true },
);
export const examDateState = atomWithStorage<Date | null>(
  "examDateState",
  null,
  undefined,
  { getOnInit: true },
);
export const staffState = atomWithStorage<StringIdNamedEntity | null>(
  "staffState",
  null,
  undefined,
  { getOnInit: true },
);
export const examMenuState = atomWithStorage<NumberIdNamedEntity[] | null>(
  "examMenuState",
  null,
  undefined,
  { getOnInit: true },
);
export const connectionEquipmentState = atomWithStorage<
  ConnectionEquipment[] | null
>("connectionEquipmentState", null, undefined, { getOnInit: true });
export const serverTimeOffsetState = atomWithStorage<number | null>(
  "serverTimeOffsetState",
  null,
  undefined,
  { getOnInit: true },
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
