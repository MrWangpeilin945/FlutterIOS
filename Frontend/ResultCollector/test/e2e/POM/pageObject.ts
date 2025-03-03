import type { Page } from "@playwright/test";
import { ExamMenuSelectPage } from "./pages/ExamMenuSelectPage";
import testSetting from "../testSetting";
import { ConsultNumberInputPage } from "./pages/ConsultNumberInputPage";
import { ExamOrderConfirmPage } from "./pages/ExamOrderConfirmPage";
import { ConsultInputPage } from "./pages/ConsultInputPage";

export class PageObject {
	readonly page: Page;

	constructor(page: Page) {
		this.page = page;
	}

	// ➀結果登録（SC0006~SC0008）
	async executeExamProcess() {
		//受診番号入力画面
		const consultNumberInputPage = new ConsultNumberInputPage(this.page);
		await consultNumberInputPage.consultNumberInput();

		//検査内容確認画面
		const examOrderConfirmPage = new ExamOrderConfirmPage(this.page);
		await examOrderConfirmPage.examOrderConfirm();

		//検査結果入力画面
		const consultInput = new ConsultInputPage(this.page);
		await consultInput.consultInput();
	}

	// ➁検査メニュー選択 + ➀結果登録を任意の回数繰り返し
	async examMenuSelectProcess() {
		//検査メニュー選択画面
		const examMenuSelectPage = new ExamMenuSelectPage(this.page);
		await examMenuSelectPage.examMenuSelect();

		//➀結果登録処理を任意の回数繰り返し
		for (let i = 0; i < testSetting.examTestLoopCount; i++) {
			console.log(`----------------- ${i + 1}回目 -----------------`);
			await this.executeExamProcess();
		}
	}
}
