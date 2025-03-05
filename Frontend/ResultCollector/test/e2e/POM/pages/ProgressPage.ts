import type { Locator, Page } from "@playwright/test";
import { BasePage } from "./base/BasePage";

export class ProgressPage extends BasePage {
  readonly placeName: Locator;

  constructor(page: Page) {
    super(page);
    this.placeName = page.getByText("性能テスト会場A");
  }

  async progress() {
    await this.goto("progress");
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await super.waitForDOMChange();
    await this.placeName.waitFor({ state: "visible" });
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0010_進捗画面", loadTime);
  }
}
