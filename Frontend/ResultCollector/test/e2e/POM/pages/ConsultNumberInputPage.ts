import testSettings from "../../testSetting";
import { BasePage } from "./base/BasePage";

export class ConsultNumberInputPage extends BasePage {
  async consultNumberInput() {
    //開始時間を計測
    const startTime = await super.getTime();
    //ページが読み込まれるまで待機
    await this.page.waitForURL(/consultnumber-input(\?.*)?$/);
    //終了時間を計測
    const endTime = await super.getTime();
    const loadTime = endTime - startTime;

    //結果をログに出力
    this.outputLog("SC0006_受診番号入力画面", loadTime);

    //検査内容確認画面へ遷移
    await this.textInput.fill(testSettings.consultNumber);
    await this.textInput.press("Enter");
  }
}
