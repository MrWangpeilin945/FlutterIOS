import { atom } from "jotai";
import type { NamedEntity, ConnectionEquipment } from "~/interfaces/interfaces";
import type { PlaceSchedule } from "~/domain/wellship.schemas";

export const teamState = atom<NamedEntity | null>(null);
export const placeScheduleState = atom<PlaceSchedule | null>();
export const examDateState = atom<Date | null>();
export const staffState = atom<NamedEntity | null>();
export const examMenuState = atom<NamedEntity[] | null>();
export const connectionEquipmentState = atom<ConnectionEquipment[] | null>();
