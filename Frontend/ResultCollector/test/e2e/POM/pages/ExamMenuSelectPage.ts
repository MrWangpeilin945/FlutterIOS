import type { Locator, Page } from "@playwright/test";
import settings from "../../testSetting";
import { BasePage } from "./base/BasePage";

export class ExamMenuSelectPage extends BasePage {
	readonly examMenuSelectButton: Locator;

	constructor(page: Page) {
		super(page);
		this.examMenuSelectButton = page.getByRole("button", {
			name: settings.examMenuName,
		});
	}

	async examMenuSelect() {
		//開始時間を計測
		const startTime = await super.getTime();
		//ページが読み込まれるまで待機
		await super.waitForDOMChange();
		//終了時間を計測
		const endTime = await super.getTime();
		const loadTime = endTime - startTime;

		//結果をログに出力
		console.log(
			`${loadTime < 3000 ? "✅" : "❌"} SC0005_検査メニュー選択画面：${loadTime.toFixed(2)}ms`,
		);

		//受診番号入力画面へ遷移
    if (!(await this.verifyButtonTextColor(this.examMenuSelectButton))) {
		await this.examMenuSelectButton.click();
    }
    await this.startButton.click();
	}
}
