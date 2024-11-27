import buyProduct from "@/services/product/buyProduct";
import refundProduct from "@/services/product/refundProduct";
import { useMutation } from "@tanstack/react-query";
import { Circle, RefreshCwIcon, ShoppingCartIcon } from "lucide-react";
import { toast } from "sonner";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "../ui/accordion";
import { Button } from "../ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";

export type Product = {
  id: string;
  productId: string;
  title: string;
  description: string;
  price: number;

  paymentStatus: string;
  ownsProduct: boolean;

  paymentObject: unknown; // TODO: Define its type
};

export default function Product({
  title,
  description,
  price,
  id: keyId,
  productId: id,
  paymentStatus,
  ownsProduct,
  paymentObject,
}: Product) {
  const { mutate: buy, isPending: isBuying } = useMutation({
    mutationFn: buyProduct,
    onSuccess: (data) => {
      window.location.href = data.url;
    },
    onError: (error) => {
      console.error(error);
      toast.error("Failed to buy product");
    },
  });

  const { mutate: refund, isPending: isRefunding } = useMutation({
    mutationFn: refundProduct,
    onSuccess: () => {
      window.location.reload();
    },
    onError: (error) => {
      console.error(error);
      toast.error("Failed to refund product");
    },
  });

  const getColorForState = (paymentStatus: string) => {
    switch (paymentStatus) {
      case "pending":
        return "fill-yellow-400 text-yellow-400";
      case "correct":
        return "fill-green-400 text-green-400";
      case "init":
        return "fill-blue-400 text-blue-400";
      case "refunded":
      case "refund":
        return "fill-red-400 text-red-400";
      default:
        return "fill-gray-400 text-gray-400";
    }
  };

  return (
    <Card className={`w-max ${ownsProduct ? "border-green-300" : ""} m-4`}>
      <CardHeader className="gap-4 space-y-0">
        <div className="space-y-1">
          <CardTitle className="flex flex-col space-y-2">
            <span>{title}</span>
            <span className="text-xl">{(price / 100).toFixed(2)} zł</span>
          </CardTitle>
          <CardDescription className="flex flex-col">
            <span>ID: {id}</span>
            <b>Key ID: {keyId}</b>
            <span>{description}</span>
          </CardDescription>
        </div>
      </CardHeader>
      <CardContent>
        {paymentObject ? (
          <Accordion type="single" collapsible className="w-full">
            <AccordionItem value="item-1">
              <AccordionTrigger>Payment details</AccordionTrigger>
              <AccordionContent>
                <code>
                  <pre>{JSON.stringify(paymentObject, null, 2)}</pre>
                </code>
              </AccordionContent>
            </AccordionItem>
          </Accordion>
        ) : null}

        <div className="flex flex-col space-y-4 mt-6 text-lg text-white">
          <div className="flex items-center">
            <Circle
              className={`mr-1 h-4 w-4 ${getColorForState(paymentStatus)}`}
            />
            {paymentStatus ?? "no payment"}
          </div>
          {ownsProduct ? (
            <Button onClick={() => refund({ id })} disabled={isRefunding}>
              <RefreshCwIcon />
              Refund
            </Button>
          ) : (
            <Button onClick={() => buy({ id })} disabled={isBuying}>
              <ShoppingCartIcon />
              Buy
            </Button>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
