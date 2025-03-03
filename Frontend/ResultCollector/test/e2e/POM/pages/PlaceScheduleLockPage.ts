import type { Locator, Page } from "@playwright/test";
import { BasePage } from "./base/BasePage";

export class PlaceScheduleLockPage extends BasePage {
  readonly unlockedButton: Locator;

  constructor(page: Page) {
    super(page);
    this.unlockedButton = page.getByRole("button", {
      name: "会場ロック解除",
    });
  }

  async getElement() {
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await super.waitForDOMChange();
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0013_会場ロック画面（初期表示）", loadTime);
  }

  async updatePlaceScheduleLockingStatus() {
    await this.unlockedButton.click();
    await this.confirmButton.click();
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await this.dialog.waitFor({ state: "visible" });
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0013_会場ロック画面（更新処理）", loadTime);
  }

  async placeScheduleLock() {
    await this.goto("placeschedule-lock");
    //初期表示
    await this.getElement();
    //会場ロック状態を更新
    await this.updatePlaceScheduleLockingStatus();
  }
}
