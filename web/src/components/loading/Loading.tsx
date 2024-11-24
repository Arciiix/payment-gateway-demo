import { cn } from "@/lib/utils";
import { Loader2 } from "lucide-react";

export default function Loading() {
  return (
    <div
      className={cn(
        "flex items-center justify-center h-screen w-full bg-gradient-to-br",
        "from-blue-900 via-gray-900 to-gray-800 text-white"
      )}
    >
      <div className="flex flex-col items-center space-y-8 px-6">
        <div className="relative">
          <Loader2 className="h-16 w-16 animate-spin text-blue-400" />
          <div className="absolute inset-0 rounded-full blur-lg bg-blue-400 opacity-20" />
        </div>

        <div className="text-center space-y-3">
          <h1 className="text-3xl font-extrabold tracking-tight sm:text-4xl">
            Loading 🚀
          </h1>
          <p className="text-sm text-gray-400">Please wait...</p>
        </div>
      </div>
    </div>
  );
}
