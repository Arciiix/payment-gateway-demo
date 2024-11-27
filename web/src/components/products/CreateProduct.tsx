import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { PlusIcon } from "lucide-react";
import { useForm } from "react-hook-form";
import {
  Form,
  FormControl,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { ProductData, productSchema } from "@/schemas/product/productSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { HTTPError } from "ky";
import { toast } from "sonner";
import { productsQuery } from "@/queries/products/products";
import addProduct from "@/services/product/addProduct";
import { useState } from "react";

export function CreateProduct() {
  const form = useForm<ProductData>({
    resolver: zodResolver(productSchema),
  });

  const queryClient = useQueryClient();

  const [isOpen, setIsOpen] = useState(false);

  const { mutate, isPending } = useMutation({
    mutationFn: addProduct,
    onError: (error) => {
      console.error(error);

      if (error instanceof HTTPError) {
        if (error.response.status === 400) {
          console.log(error.response.body);
          toast.error("Validation error");
          return;
        }
      }

      toast.error("An error occurred while trying to add a new product.");
    },
    onSuccess: () => {
      toast.success("Added a new product!");

      queryClient.invalidateQueries({ queryKey: productsQuery.queryKey });

      setIsOpen(false);
    },
  });

  const onSubmit = (data: ProductData) => {
    mutate(data);
  };

  return (
    <Dialog open={isOpen} onOpenChange={(open) => setIsOpen(open)}>
      <DialogTrigger asChild>
        <Button variant="outline" size="icon">
          <PlusIcon />
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Create product</DialogTitle>
          <DialogDescription>Add a new product</DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="grid gap-4 py-4"
          >
            <FormItem className="w-full">
              <FormLabel htmlFor="title" className="text-right">
                Title
              </FormLabel>
              <FormControl>
                <Input id="title" {...form.register("title")} />
              </FormControl>
              <FormMessage>{form.formState.errors.title?.message}</FormMessage>
            </FormItem>
            <FormItem>
              <FormLabel htmlFor="description" className="text-right">
                Description
              </FormLabel>
              <FormControl>
                <Input
                  id="description"
                  min={0}
                  {...form.register("description")}
                />
              </FormControl>
              <FormMessage>
                {form.formState.errors.description?.message}
              </FormMessage>
            </FormItem>
            <FormItem>
              <FormLabel htmlFor="price" className="text-right">
                Price
              </FormLabel>
              <FormControl>
                <Input
                  id="price"
                  type="number"
                  min={0}
                  step={0.01}
                  {...form.register("price", { valueAsNumber: true })}
                />
              </FormControl>
              <FormMessage>{form.formState.errors.price?.message}</FormMessage>
            </FormItem>
            <DialogFooter>
              <Button type="submit" disabled={isPending}>
                Add
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
