import { api } from "@/lib/fetcher";
import { BillingDictionary } from "@/types/billing/billing";

export default async function fetchBillings(): Promise<BillingDictionary> {
  try {
    const response = await api.get<BillingDictionary>("/api/product/billings");
    return response.json();
  } catch (error) {
    console.log("fetchBillings error");
    console.error(error);
    return {};
  }
}
