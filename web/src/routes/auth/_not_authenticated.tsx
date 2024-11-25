import { userQuery } from "@/queries/auth/user";
import { useQuery } from "@tanstack/react-query";
import {
  createFileRoute,
  Outlet,
  redirect,
  useNavigate,
} from "@tanstack/react-router";
import { useEffect } from "react";

export const Route = createFileRoute("/auth/_not_authenticated")({
  beforeLoad: ({ context }) => {
    console.log(context.user);
    if (context.user) {
      throw redirect({
        to: "/",
      });
    }
  },
  component: NotAuthenticatedComponent,
});

export function NotAuthenticatedComponent() {
  const { data: user } = useQuery(userQuery);
  const navigate = useNavigate();

  useEffect(() => {
    if (user) {
      console.log("User is logged in, navigate to /");

      navigate({
        to: "/",
      });
    }
  }, [navigate, user]);

  return <Outlet />;
}
