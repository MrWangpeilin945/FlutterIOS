import type { Locator, Page } from "@playwright/test";
import { BasePage } from "./base/BasePage";

export class HomePage extends BasePage {
  readonly homeMenuButton: Locator;

  constructor(page: Page) {
    super(page);
    this.homeMenuButton = page.getByRole("button", { name: "進捗" });
  }

  async home() {
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await this.homeMenuButton.waitFor({ state: "visible" });
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0004_ホーム画面", loadTime);
  }
}
