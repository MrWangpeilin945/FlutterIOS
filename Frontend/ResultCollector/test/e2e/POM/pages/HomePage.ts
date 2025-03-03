import { BasePage } from "./base/BasePage";

export class HomePage extends BasePage {
  async home() {
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await super.waitForDOMChange();
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
		console.log(
			`${loadTime < 3000 ? "✅" : "❌"} SC0004_ホーム画面：${loadTime.toFixed(2)}ms`,
		);
  }
}
