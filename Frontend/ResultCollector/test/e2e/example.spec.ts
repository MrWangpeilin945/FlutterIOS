import { test } from "@playwright/test";
import testSetting from "./testSetting";
import { PageObject } from "./POM/pageObject";
import { Login } from "./POM/pages/LoginPage";
import { TeamSelectPage } from "./POM/pages/TeamSelectPage";
import { PlaceSelectPage } from "./POM/pages/PlaceSelectPage";
import { HomePage } from "./POM/pages/HomePage";
import { ProgressPage } from "./POM/pages/ProgressPage";
import { PlaceScheduleLockPage } from "./POM/pages/PlaceScheduleLockPage";
import { ExamResultExportPage } from "./POM/pages/ExamResultExportPage";
import { ExamResultExportHistoryPage } from "./POM/pages/ExamResultExportHistoryPage";

test.beforeEach(async ({ page }) => {
  const loginPage = new Login(page);
  await loginPage.login();
});

test("シナリオ➀", async ({ page }) => {
  const pageObject = new PageObject(page);

  //班選択画面
  const teamSelectPage = new TeamSelectPage(page);
  await teamSelectPage.teamSelect();

  //会場選択画面
  const placeSelectPage = new PlaceSelectPage(page);
  await placeSelectPage.placeSelect();

  //ホーム画面
  const homePage = new HomePage(page);
  await homePage.home();

  //任意の回数繰り返し（検査メニュー選択～検査結果入力）
  for (let i = 0; i < testSetting.examMenuTestLoopCount; i++) {
    console.log(`------------------- ${i + 1}回目 -------------------`);
    await homePage.homeMenuButton.waitFor({ state: "visible" });
    await pageObject.examMenuSelectProcess();
  }
});

test("シナリオ➁", async ({ page }) => {
  const targetTeamSelectButton = new TeamSelectPage(page).teamSelectButton;
  const targetPlaceSelectButton = new PlaceSelectPage(page).startTime;
  //班を選択
  await targetTeamSelectButton.click();
  await page.waitForURL("place-select");
  //会場を選択
  await targetPlaceSelectButton.click();

  await page.waitForURL("home");

  //進捗画面
  const progressPage = new ProgressPage(page);
  //任意の回数繰り返し
  for (let i = 0; i < testSetting.progressTestLoopCount; i++) {
    await progressPage.progress();
  }

  //会場ロック画面
  const placeScheduleLockPage = new PlaceScheduleLockPage(page);
  await placeScheduleLockPage.placeScheduleLock();

  //検査結果出力画面
  const examResultExportPage = new ExamResultExportPage(page);
  await examResultExportPage.examResultExport();

  //検査結果出力履歴画面
  const examResultExportHistory = new ExamResultExportHistoryPage(page);
  await examResultExportHistory.examResultExportHistory();
});
