import { Product } from "@/components/products/Product";
import { api } from "@/lib/fetcher";
import { ProductData } from "@/schemas/product/productSchema";

export default async function addProduct(
  product: ProductData
): Promise<Product> {
  try {
    const response = await api.post<{ url: string }>(`/api/product`, {
      json: product,
    });
    return response.json();
  } catch (error) {
    console.log("addProduct error", error);

    throw error;
  }
}
