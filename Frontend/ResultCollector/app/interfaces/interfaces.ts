import type { Equipment } from "~/domain/wellship.schemas";

/* IDと名前を持つ基本的なエンティティ(IDがstring) */
export interface StringIdNamedEntity {
  /** ID */
  id?: string;
  /** 名前 */
  name?: string;
}

/* IDと名前を持つ基本的なエンティティ(IDがnumber) */
export interface NumberIdNamedEntity {
  /** ID */
  id?: number;
  /** 名前 */
  name?: string;
}

/* 選択している検査機器 */
export interface ConnectionEquipment {
  /** 検査メニューID */
  examMenuId: number;
  /** 検査機器情報 */
  equipment: Equipment | null;
}
