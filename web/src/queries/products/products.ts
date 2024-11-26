import fetchProducts from "@/services/product/fetchProducts";
import { queryOptions } from "@tanstack/react-query";

export const productsQuery = queryOptions({
  queryKey: ["products"],
  queryFn: fetchProducts,
  gcTime: Infinity,
  refetchOnWindowFocus: false,
});
