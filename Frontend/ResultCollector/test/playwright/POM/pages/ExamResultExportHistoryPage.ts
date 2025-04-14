import { BasePage } from "./base/BasePage";

export class ExamResultExportHistoryPage extends BasePage {
  async examResultExportHistory() {
    await this.goto("examresult-export-history");
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await super.waitForDOMChange();
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0020_検査結果出力履歴画面", loadTime);

    //TODO:未出力に変更時の計測
  }
}
