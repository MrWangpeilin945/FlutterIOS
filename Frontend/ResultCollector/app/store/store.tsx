import { atom } from "jotai";
import type {
  StringIdNamedEntity,
  NumberIdNamedEntity,
  ConnectionEquipment,
} from "~/interfaces/interfaces";
import type { PlaceSchedule } from "~/domain/wellship.schemas";

export const teamState = atom<StringIdNamedEntity | null>(null);
export const placeScheduleState = atom<PlaceSchedule | null>(null);
export const examDateState = atom<Date | null>(null);
export const staffState = atom<StringIdNamedEntity | null>(null);
export const examMenuState = atom<NumberIdNamedEntity[] | null>(null);
export const connectionEquipmentState = atom<ConnectionEquipment[] | null>(
  null,
);
export const serverTimeOffsetState = atom<number | null>(null);

// 状態管理をクリアする(ログアウト処理で使用)
export const clearAllState = atom(null, (_get, set) => {
  set(teamState, null);
  set(placeScheduleState, null);
  set(examDateState, null);
  set(staffState, null);
  set(examMenuState, null);
  set(connectionEquipmentState, null);
  set(serverTimeOffsetState, null);
});
