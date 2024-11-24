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
  beforeLoad: ({ context }) => {
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

  const user = useQuery(userQuery);

  useEffect(() => {
    navigate({
      to: "/auth/login",
    });
  }, [user, navigate]);

  return <Outlet />;
}
