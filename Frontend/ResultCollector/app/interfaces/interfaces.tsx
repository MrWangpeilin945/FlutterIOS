import type { Equipment } from "~/domain/wellship.schemas";

/* IDと名前を持つ基本的なエンティティ */
export interface NamedEntity {
  /** ID */
  id?: number;
  /** 名前 */
  name?: string;
}

/* 選択している検査機器 */
export interface ConnectionEquipment {
  examMenuId: number;
  equipment: Equipment | null;
}
