import { atom } from 'jotai';
import type { NamedEntity } from '~/interfaces/interfaces';
import type { Place, PlaceScheduleTeam } from '~/domain/wellship.schemas';

export const placeState = atom<Place | null>(null);
export const teamAtom = atom<NamedEntity | null>(null);
export const placeScheduleState = atom<PlaceScheduleTeam | null>();
export const examDateState = atom<Date | null>();
export const staffState = atom<NamedEntity | null >();