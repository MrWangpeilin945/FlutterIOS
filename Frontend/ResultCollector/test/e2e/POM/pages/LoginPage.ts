import type { Locator, Page } from "@playwright/test";
import settings from "../../testSetting";
import { BasePage } from "./base/BasePage";

export class Login extends BasePage {
	readonly loginButton: Locator;


	constructor(page: Page) {
		super(page);
		this.loginButton = page.getByRole("button", { name: "ログイン" });
	}

	// ログイン処理
	async loginProcess() {
		await this.textInput.fill(settings.loginId);
		await this.passwordInput.click();
		await this.passwordInput.fill(settings.password);
		await this.loginButton.click();
	}

	async login() {
		//ログイン画面へ遷移
		await super.goto("login");

		//開始時間を計測
		const startTime = await super.getTime();
		//ページが読み込まれるまで待機
		await super.waitForDOMChange();
		//終了時間を計測
		const endTime = await super.getTime();
		const loadTime = endTime - startTime;

		//結果をログに出力
		console.log(
			`${loadTime < 3000 ? "✅" : "❌"} SC0001_ログイン画面：${loadTime.toFixed(2)}ms`,
		);

		//班選択画面へ遷移
		await this.loginProcess();
	}
}
