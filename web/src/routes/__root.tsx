import { Toaster } from "@/components/ui/sonner";
import { queryClient } from "@/main";
import { userQuery } from "@/queries/auth/user";
import { User } from "@/types/auth/user";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/router-devtools";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import Loading from "@/components/loading/Loading";

type MyRouterContext = {
  user: User | null;
};

export const Route = createRootRouteWithContext<MyRouterContext>()({
  beforeLoad: async () => await queryClient.ensureQueryData(userQuery),
  wrapInSuspense: true,
  pendingComponent: () => <Loading />,
  component: () => (
    <>
      <Outlet />
      <Toaster />
      <TanStackRouterDevtools />
      <ReactQueryDevtools initialIsOpen={false} />
    </>
  ),
});
