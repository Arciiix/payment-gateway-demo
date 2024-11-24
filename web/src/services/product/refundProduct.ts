import { api } from "@/lib/fetcher";

export default async function refundProduct({
  id,
}: {
  id: string;
}): Promise<void> {
  try {
    await api.post(`/api/product/${id}/refund`);
  } catch (error) {
    console.log("refundProduct error");

    throw error;
  }
}
