"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import type { Role } from "@/lib/types";
import { Button } from "@/components/ui/Button";

interface NavItem {
  href: string;
  label: string;
  roles: Role[];
}

const NAV: NavItem[] = [
  { href: "/dashboard", label: "Dashboard", roles: ["Admin", "Teacher", "Student"] },
  { href: "/assignments", label: "Assignments", roles: ["Admin", "Teacher", "Student"] },
  { href: "/admin/users", label: "Users", roles: ["Admin"] },
  { href: "/admin/classes", label: "Classes", roles: ["Admin"] },
  { href: "/admin/subjects", label: "Subjects", roles: ["Admin"] },
  { href: "/admin/teacher-assignments", label: "Teacher Assignments", roles: ["Admin"] },
  { href: "/admin/enrollments", label: "Enrollments", roles: ["Admin"] },
];

export function Sidebar({
  open,
  onClose,
}: {
  open: boolean;
  onClose: () => void;
}) {
  const { user, logout } = useAuth();
  const pathname = usePathname();

  const items = NAV.filter((item) => user && item.roles.includes(user.role));

  return (
    <>
      {open && (
        <button
          type="button"
          className="fixed inset-0 z-30 bg-slate-900/40 lg:hidden"
          aria-label="Close menu"
          onClick={onClose}
        />
      )}
      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-72 flex-col border-r border-slate-200 bg-white/95 backdrop-blur transition-transform lg:static lg:translate-x-0 ${
          open ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        <div className="border-b border-slate-100 px-5 py-5">
          <p className="font-display text-xl font-semibold text-teal-800">
            CampusDesk
          </p>
          <p className="mt-0.5 text-xs text-slate-500">
            Assignment & Submission System
          </p>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-3 py-4">
          {items.map((item) => {
            const active =
              pathname === item.href ||
              (item.href !== "/dashboard" && pathname.startsWith(item.href));
            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={onClose}
                className={`block rounded-md px-3 py-2 text-sm font-medium transition ${
                  active
                    ? "bg-teal-700 text-white"
                    : "text-slate-700 hover:bg-slate-100"
                }`}
              >
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div className="border-t border-slate-100 px-4 py-4">
          <div className="mb-3">
            <p className="truncate text-sm font-medium text-slate-900">
              {user?.fullName}
            </p>
            <p className="truncate text-xs text-slate-500">{user?.email}</p>
            <p className="mt-1 text-xs font-medium text-teal-700">{user?.role}</p>
          </div>
          <Button variant="secondary" className="w-full" onClick={logout}>
            Log out
          </Button>
        </div>
      </aside>
    </>
  );
}
