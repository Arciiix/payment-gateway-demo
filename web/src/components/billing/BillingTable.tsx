import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { billingsQuery } from "@/queries/billings/billings";
import { Billing, BillingDictionary } from "@/types/billing/billing";
import { useQuery } from "@tanstack/react-query";

export default function ProductsTable() {
  const { data } = useQuery<BillingDictionary>(billingsQuery);

  const hasData = (key: string) =>
    Array.isArray(data![key]) && data![key].length > 0;

  if (!data) return null;
  return (
    <div className="space-y-8">
      {Object.keys(data).map((key) => {
        if (!hasData(key)) return null;

        return (
          <div key={key}>
            <h2 className="text-lg font-semibold mb-4 capitalize">{key}</h2>
            <Table className="w-full border rounded-md">
              <TableHeader>
                <TableRow>
                  {Object.keys(data[key][0]).map((column) => (
                    <TableHead key={column} className="capitalize">
                      {column}
                    </TableHead>
                  ))}
                </TableRow>
              </TableHeader>
              <TableBody>
                {data[key].map((row: Billing, rowIndex: number) => (
                  <TableRow key={rowIndex}>
                    {Object.values(row).map((cell, cellIndex) => (
                      <TableCell key={cellIndex} className="whitespace-nowrap">
                        {cell}
                      </TableCell>
                    ))}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        );
      })}
    </div>
  );
}
