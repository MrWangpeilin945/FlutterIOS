import type { MetaFunction } from "@remix-run/node";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import Header from "~/components/header";
import HanList from "~/components/routes/han/hanList";

export const meta: MetaFunction = () => {
	return [{ title: "班選択" }];
};

const queryClient = new QueryClient();

export default function selectHan() {
	return (
		<div>
			<Header title="班選択" />
			<QueryClientProvider client={queryClient}>
				<HanList />
			</QueryClientProvider>
		</div>
	);
}
