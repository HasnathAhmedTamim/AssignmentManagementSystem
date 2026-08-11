"use client";

import { useState } from "react";
import { Sidebar } from "./Sidebar";
import { Button } from "@/components/ui/Button";

export function AppShell({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useState(false);

  return (
    <div className="flex min-h-screen bg-[radial-gradient(ellipse_at_top,_#f1f5f9_0%,_#e2e8f0_45%,_#f8fafc_100%)]">
      <Sidebar open={open} onClose={() => setOpen(false)} />
      <div className="flex min-w-0 flex-1 flex-col">
        <header className="sticky top-0 z-20 flex items-center gap-3 border-b border-slate-200/80 bg-white/80 px-4 py-3 backdrop-blur lg:hidden">
          <Button variant="secondary" onClick={() => setOpen(true)}>
            Menu
          </Button>
          <span className="font-display font-semibold text-teal-800">
            CampusDesk
          </span>
        </header>
        <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-6 sm:px-6 lg:px-8">
          {children}
        </main>
      </div>
    </div>
  );
}
