import fetchBillings from "@/services/billings/fetchBillings";
import { queryOptions } from "@tanstack/react-query";

export const billingsQuery = queryOptions({
  queryKey: ["billings"],
  queryFn: fetchBillings,
  gcTime: Infinity,
});
