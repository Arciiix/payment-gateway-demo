import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@radix-ui/react-accordion";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";
import { Skeleton } from "../ui/skeleton";

export default function ProductSkeleton() {
  return (
    <Card className="w-max border-gray-300">
      <CardHeader className="gap-4 space-y-0">
        <div className="space-y-1">
          <CardTitle className="flex flex-col space-y-2">
            <Skeleton className="h-6 w-32" />
            <Skeleton className="h-6 w-20" />
          </CardTitle>
          <CardDescription className="flex flex-col">
            <Skeleton className="h-4 w-24" />
            <Skeleton className="h-4 w-full" />
          </CardDescription>
        </div>
      </CardHeader>
      <CardContent>
        <Accordion type="single" collapsible className="w-full">
          <AccordionItem value="item-1">
            <AccordionTrigger>
              <Skeleton className="h-4 w-32" />
            </AccordionTrigger>
            <AccordionContent>
              <Skeleton className="h-24 w-full" />
            </AccordionContent>
          </AccordionItem>
        </Accordion>

        <div className="flex flex-col space-y-4 mt-6 text-lg text-white">
          <div className="flex items-center">
            <Skeleton className="h-4 w-4 mr-1" />
            <Skeleton className="h-4 w-20" />
          </div>
          <Skeleton className="h-10 w-32" />
        </div>
      </CardContent>
    </Card>
  );
}
