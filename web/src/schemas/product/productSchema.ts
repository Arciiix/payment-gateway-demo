import { z } from "zod";

export const productSchema = z.object({
  title: z.string().min(1, "Title cannot be empty"),
  description: z.string().min(1, "Description cannot be empty"),
  price: z.number().min(0.01, "Price must be greater than 0"),
});

export type ProductData = z.infer<typeof productSchema>;
