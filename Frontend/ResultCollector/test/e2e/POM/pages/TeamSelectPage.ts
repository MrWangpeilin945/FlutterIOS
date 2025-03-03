import type { Locator, Page } from "@playwright/test";
import settings from "../../testSetting";
import { BasePage } from "./base/BasePage";

export class TeamSelectPage extends BasePage {
	readonly teamSelectButton: Locator;

	constructor(page: Page) {
		super(page);
		this.teamSelectButton = page.getByRole("button", {
			name: settings.teamName,
		});
	}

	async teamSelect() {
		//開始時間を計測
		const startTime = await super.getTime();
		//ページが読み込まれるまで待機
		await super.waitForDOMChange();
		//終了時間を計測
		const endTime = await super.getTime();
		const loadTime = endTime - startTime;

		//結果をログに出力
		console.log(
			`${loadTime < 3000 ? "✅" : "❌"} SC0002_班選択画面：${loadTime.toFixed(2)}ms`,
		);

		//会場選択画面へ遷移
		await this.teamSelectButton.click();
	}
}
