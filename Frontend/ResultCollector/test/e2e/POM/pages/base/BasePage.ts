import type { Locator, Page } from "@playwright/test";
import testSettings from "test/e2e/testSetting";

export class BasePage {
  readonly page: Page;
  readonly textInput: Locator;
  readonly passwordInput: Locator;
  readonly startButton: Locator;
  readonly registerButton: Locator;
  readonly confirmButton: Locator;
  readonly dialog: Locator;

  constructor(page: Page) {
    this.page = page;
    this.textInput = page.locator('input[class*="mantine-TextInput-input"]');
    this.passwordInput = page.locator('input[type="password"]');
    this.startButton = page.getByRole("button", { name: "開始する" });
    this.registerButton = page.getByRole("button", { name: "登録する" });
    this.confirmButton = page.getByRole("button", { name: "OK" });
    this.dialog = page.locator('div[class*="mantine-Modal-body"]');
  }

  async outputLog(name: string, loadTime: number) {
    console.log(
      `${loadTime < testSettings.baseProcessingTime ? "✅" : "❌"} ${name}：${loadTime.toFixed(2)}ms`,
    );
  }

  async goto(url: string) {
    await this.page.goto(url);
  }

  async waitForNetworkIdle() {
    await this.page.waitForLoadState("networkidle");
  }

  async takeScreenshot(path: string) {
    await this.page.screenshot({ path, fullPage: true });
  }

  async getTime() {
    return await this.page.evaluate(() => performance.now());
  }

  async waitUrl(url: string | RegExp) {
    return await this.page.waitForURL(url);
  }

  // DOM の変化を待機するメソッド
  async waitForDOMChange() {
    await this.page.evaluate(() => {
      return new Promise<void>((resolve) => {
        const observer = new MutationObserver(() => {
          observer.disconnect();
          resolve();
        });
        observer.observe(document.body, { childList: true });
      });
    });
  }

  // ボタンの文字色が期待値と一致するかチェック
  async verifyButtonTextColor(targetbutton: Locator) {
    const textColor = await targetbutton.evaluate(
      (el) => window.getComputedStyle(el).color,
    );
    //primaryカラーと比較
    return textColor === "rgb(0, 151, 154)";
  }
}
