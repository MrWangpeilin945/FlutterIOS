import '@mantine/core/styles.css';
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  useNavigate,
} from '@remix-run/react';
import { ColorSchemeScript, MantineProvider } from '@mantine/core';
import { useEffect } from 'react';
import { setupAxiosInterceptors } from '~/utils/axiosInstance';

const queryClient = new QueryClient();

export function Layout({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate();

  const redirectToLogin = () => {
    navigate('/login');
  };

  useEffect(() => {
    // API呼び出し時に401が返ってきたらログイン画面に遷移する処理をaxiosInstanceに引き渡す
    setupAxiosInterceptors(redirectToLogin);
  }, []);

  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1"/>
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
        <QueryClientProvider client={queryClient}>
          {children}
        </QueryClientProvider>
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
