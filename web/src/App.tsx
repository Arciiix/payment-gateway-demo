import { createRouter, RouterProvider } from "@tanstack/react-router";
import { routeTree } from "./routeTree.gen";
import { useQuery } from "@tanstack/react-query";
import { userQuery } from "./queries/auth/user";

const router = createRouter({
  routeTree,
  context: {
    user: undefined!,
  },
});
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

export default function App() {
  const user = useQuery(userQuery);
  return <RouterProvider router={router} context={{ user: user.data }} />;
}
