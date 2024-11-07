import { setupWorker } from 'msw/browser'
import { getWellshipMock } from "~/api/wellship.msw";
export const worker = setupWorker(...getWellshipMock())