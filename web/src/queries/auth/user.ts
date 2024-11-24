import fetchUser from "@/services/auth/fetchUser";
import { queryOptions } from "@tanstack/react-query";

export const userQuery = queryOptions({
  queryKey: ["currentUser"],
  queryFn: fetchUser,
  gcTime: Infinity,
});
