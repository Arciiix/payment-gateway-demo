import { createLazyFileRoute } from "@tanstack/react-router";

export const Route = createLazyFileRoute("/_authenticated/")({
  component: RouteComponent,
});

function RouteComponent() {
  return "Hello /app/!";
}
