import { api } from "@/lib/fetcher";

export default async function buyProduct({
  id,
}: {
  id: string;
}): Promise<{ url: string }> {
  try {
    const response = await api.post<{ url: string }>(`/api/product/${id}/buy`);
    return response.json();
  } catch (error) {
    console.log("buyProduct error", error);

    throw error;
  }
}
