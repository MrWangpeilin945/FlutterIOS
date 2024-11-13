/**
 * By default, Remix will handle hydrating your app on the client for you.
 * You are free to delete this file if you'd like to, but if you ever want it revealed again, you can run `npx remix reveal` ✨
 * For more information, see https://remix.run/file-conventions/entry.client
 */

import { RemixBrowser } from "@remix-run/react";
import { startTransition, StrictMode } from "react";
import { hydrateRoot } from "react-dom/client";

//mock
import { setupWorker } from "msw/browser";
import { getWellshipMock } from "~/api/wellship.msw";

async function prepareApp() {
  if (process.env.NODE_ENV === "development") {
    const worker = setupWorker(...getWellshipMock());
    return worker.start({
      serviceWorker: {
        url: "/ResultCollector/mockServiceWorker.js",
      },
      onUnhandledRequest: (req) => {
        console.warn("Unhandled request:", req);
      },
    });
  }
  return Promise.resolve();
}

prepareApp().then(() => {
  startTransition(() => {
    hydrateRoot(
      document,
      <StrictMode>
        <RemixBrowser />
      </StrictMode>
    );
  });
});
