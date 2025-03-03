import { BasePage } from "./base/BasePage";

export class ProgressPage extends BasePage {
	async progress() {
    await this.goto("progress");
		//開始時間を計測
		const startTime = await super.getTime();
		//ページが読み込まれるまで待機
		await super.waitForDOMChange();
		//終了時間を計測
		const endTime = await super.getTime();
		const loadTime = endTime - startTime;

		//結果をログに出力
		console.log(
			`${loadTime < 3000 ? "✅" : "❌"} SC0010_進捗画面：${loadTime.toFixed(2)}ms`,
		);
	}
}
