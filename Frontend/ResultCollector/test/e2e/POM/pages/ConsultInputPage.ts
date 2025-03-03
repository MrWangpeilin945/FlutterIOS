import type { Locator, Page } from "@playwright/test";
import { BasePage } from "./base/BasePage";

export class ConsultInputPage extends BasePage {
  readonly leftSelectButton: Locator;
  readonly rightSelectButton: Locator;

  constructor(page: Page) {
    super(page);
    this.leftSelectButton = page.getByRole("button", { name: "左選択肢B" });
    this.rightSelectButton = page.getByRole("button", { name: "右選択肢A" });
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
    this.outputLog("SC0008_検査結果入力画面（初期表示）", loadTime);
  }

  async verifyExamItems() {
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await this.dialog.waitFor({ state: "visible" });
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0008_検査結果入力画面（検証処理）", loadTime);
  }

  async consultInput() {
    //初期表示
    await this.getElement();

    //結果を入力
    await this.textInput.nth(0).click();
    await this.textInput.nth(0).fill("150");
    if (!(await this.verifyButtonTextColor(this.leftSelectButton))) {
      await this.leftSelectButton.click();
    }
    if (!(await this.verifyButtonTextColor(this.rightSelectButton))) {
      await this.rightSelectButton.click();
    }
    await this.registerButton.click();

    //検査結果を検証
    await this.verifyExamItems();

    //受診番号入力画面へ遷移
    await this.confirmButton.click();
  }
}
