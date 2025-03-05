import type { Locator, Page } from "@playwright/test";
import settings from "../../testSetting";
import { BasePage } from "./base/BasePage";

export class PlaceSelectPage extends BasePage {
  readonly startTime: Locator;

  constructor(page: Page) {
    super(page);
    this.startTime = page.getByRole("button", {
      name: settings.startTime,
    });
  }

  async placeSelect() {
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await this.startTime.waitFor({state:"visible"});
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0003_会場選択画面", loadTime);

    //ホーム画面へ遷移
    await this.startTime.click();
  }
}
