import { Product } from "@/components/products/Product";
import { api } from "@/lib/fetcher";

export default async function fetchProductsRefreshed(): Promise<Product[]> {
  try {
    console.log("fetchProductsRefreshed");
    const response = await api.get<Product[]>("/api/product/new");
    return response.json();
  } catch (error) {
    console.log("fetchProductsRefreshed error");
    console.error(error);
    return [];
  }
}
