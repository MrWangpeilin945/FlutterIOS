import { atom } from 'jotai';
import type { Place, PlaceScheduleTeam } from '~/api/wellship.schemas';

export const placeState = atom<Place | null>(null);
export const teamAtom = atom<{teamId:number,teamName:string} | null>(null);
export const placeScheduleState = atom<PlaceScheduleTeam | null>();
export const examDateState = atom<Date | null>();
export const staffState = atom<{staffId:number,stafName:string} | null >();