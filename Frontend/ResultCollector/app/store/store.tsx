import { atom } from 'jotai';
import type { NamedEntity } from '~/interfaces/interfaces';
import type { PlaceScheduleTeam } from '~/domain/wellship.schemas';

export const teamState = atom<NamedEntity | null>(null);
export const placeScheduleState = atom<PlaceScheduleTeam | null>();
export const examDateState = atom<Date | null>();
export const staffState = atom<NamedEntity | null >();
export const examMenuState = atom<NamedEntity[] | null>();