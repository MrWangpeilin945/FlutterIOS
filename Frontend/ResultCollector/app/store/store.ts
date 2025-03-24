import { atom } from "jotai";
import { atomWithStorage, createJSONStorage, RESET } from "jotai/utils";
import type {
  StringIdNamedEntity,
  NumberIdNamedEntity,
  ConnectionEquipment,
} from "~/interfaces/interfaces";
import type { PlaceSchedule } from "~/domain/wellship.schemas";

// 状態管理はすべてsessionStorageに保存する(atomWithStorage)
export const teamState = atomWithStorage<StringIdNamedEntity | null>(
  "teamState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<StringIdNamedEntity | null>(() => window.sessionStorage)
    : undefined,
  { getOnInit: true },
);
export const placeScheduleState = atomWithStorage<PlaceSchedule | null>(
  "placeScheduleState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<PlaceSchedule | null>(() => window.sessionStorage)
    : undefined,
  { getOnInit: true },
);
export const examDateState = atomWithStorage<Date | null>(
  "examDateState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<Date | null>(() => window.sessionStorage)
    : undefined,
  { getOnInit: true },
);
export const staffState = atomWithStorage<StringIdNamedEntity | null>(
  "staffState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<StringIdNamedEntity | null>(() => window.sessionStorage)
    : undefined,
  { getOnInit: true },
);
export const examMenuState = atomWithStorage<NumberIdNamedEntity[] | null>(
  "examMenuState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<NumberIdNamedEntity[] | null>(
        () => window.sessionStorage,
      )
    : undefined,
  { getOnInit: true },
);
export const connectionEquipmentState = atomWithStorage<
  ConnectionEquipment[] | null
>(
  "connectionEquipmentState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<ConnectionEquipment[] | null>(
        () => window.sessionStorage,
      )
    : undefined,
  { getOnInit: true },
);
export const serverTimeOffsetState = atomWithStorage<number | null>(
  "serverTimeOffsetState",
  null,
  typeof window !== "undefined"
    ? createJSONStorage<number | null>(() => window.sessionStorage)
    : undefined,
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

// 状態管理をクリアする(班選択画面で使用)
export const clearExamState = atom(null, (_get, set) => {
  set(placeScheduleState, RESET);
  set(examDateState, RESET);
  set(examMenuState, RESET);
});
