import { queryClient } from "@/main";
import { userQuery } from "@/queries/auth/user";
import { useQuery } from "@tanstack/react-query";
import {
  createFileRoute,
  Outlet,
  redirect,
  useNavigate,
} from "@tanstack/react-router";
import { useEffect } from "react";

export const Route = createFileRoute("/_authenticated")({
  beforeLoad: async ({ context }) => {
    await queryClient.ensureQueryData(userQuery);
    if (context.user === null) {
      throw redirect({
        to: "/auth/login",
      });
    }
  },
  component: AuthenticatedComponent,
});

export function AuthenticatedComponent() {
  const navigate = useNavigate();

  const { data: user } = useQuery(userQuery);

  useEffect(() => {
    if (!user) {
      console.log("No user, navigate to login...");
      navigate({
        to: "/auth/login",
      });
    }
  }, [user, navigate]);

  return <Outlet />;
}
