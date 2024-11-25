import * as React from "react";
import { createLazyFileRoute, Link } from "@tanstack/react-router";
import { CheckIcon } from "lucide-react";
import { Button } from "@/components/ui/button";

export const Route = createLazyFileRoute("/_authenticated/payment/success")({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-black p-4">
      <div className="bg-gray-800 rounded-lg shadow-lg p-6 max-w-md w-full">
        <div className="flex flex-col items-center">
          <div className="bg-green-100 rounded-full p-3 mb-4">
            <CheckIcon size={32} color="black" />
          </div>
          <h1 className="text-2xl font-semibold text-white mb-2">
            Payment Successful
          </h1>
          <p className="text-gray-100 mb-4 text-center">
            Thank you for your payment. Your transaction has been completed
            successfully.
          </p>
          <Link to="/">
            <Button>Go to dashboard</Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
