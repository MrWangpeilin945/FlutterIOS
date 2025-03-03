import { BasePage } from "./base/BasePage";

export class ExamOrderConfirmPage extends BasePage {
	async examOrderConfirm() {
		//開始時間を計測
		const startTime = await super.getTime();
		//ページが読み込まれるまで待機
		await super.waitForDOMChange();
		//終了時間を計測
		const endTime = await super.getTime();
		const loadTime = endTime - startTime;

		//結果をログに出力
		console.log(
			`${loadTime < 3000 ? "✅" : "❌"} SC0007_検査内容確認画面：${loadTime.toFixed(2)}ms`,
		);

		//検査結果入力画面へ遷移
		await this.startButton.click();
	}
}
