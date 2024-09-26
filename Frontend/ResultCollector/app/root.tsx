import '@mantine/core/styles.css';

import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
} from '@remix-run/react';
import { ColorSchemeScript, MantineProvider } from '@mantine/core';

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta
          name="viewport"
          content="width=device-width, initial-scale=1"
        />
        <Meta />
        <Links />
        <ColorSchemeScript />
      </head>
      <body>
      <MantineProvider
            theme={{
                components: {
                    Button: {
                        styles: {
                            root: {
                                boxShadow: '3px 3px 4px rgba(0, 0, 0, 0.3)', 
                                transition: 'all 0.3s ease', 
                                '&:hover': {
                                    boxShadow: '5px 5px 10px rgba(0, 0, 0, 0.4)', 
                                },
                            },
                        },
                    },
                },
            }}
        >
          {children}
        </MantineProvider>
        <ScrollRestoration />
        <Scripts />
      </body>
    </html>
  );
}

export default function App() {
  return <Outlet />;
}

export function HydrateFallback() {
  return <p>Loading...</p>;
}
