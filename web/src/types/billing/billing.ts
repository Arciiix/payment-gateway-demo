export type Billing = {
  transactionId: string;
  productKeyId: string;
  title: string;
  friendlyTitle: string;
  price: number;
  status: string;
  userId: string;
  creationDate: string;
  realizationDate?: string | null;
};

export type BillingDictionary = Record<string, Billing[]>;
