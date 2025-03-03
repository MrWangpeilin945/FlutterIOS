import { BasePage } from "./base/BasePage";

export class ExamResultExportPage extends BasePage {
  async examResultExport() {
    await this.goto("examresult-export");
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await super.waitForDOMChange();
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0019_検査結果出力画面", loadTime);

    //TODO:出力時の計測
  }
}
