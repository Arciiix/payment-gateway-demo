import { Product } from "@/components/products/Product";
import { api } from "@/lib/fetcher";

export default async function fetchProducts(): Promise<Product[]> {
  try {
    const response = await api.get<Product[]>("/api/product");
    return response.json();
  } catch (error) {
    console.log("fetchProducts error");
    console.error(error);
    return [];
  }
}
