"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import type { Role } from "@/lib/types";
import { Spinner } from "@/components/ui/Spinner";

export function ProtectedRoute({
  children,
  roles,
}: {
  children: React.ReactNode;
  roles?: Role[];
}) {
  const { user, loading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (loading) return;
    if (!user) {
      router.replace("/login");
      return;
    }
    if (roles && !roles.includes(user.role)) {
      router.replace("/dashboard");
    }
  }, [user, loading, roles, router]);

  if (loading) return <Spinner />;
  if (!user) return <Spinner label="Redirecting to login..." />;
  if (roles && !roles.includes(user.role)) {
    return <Spinner label="Redirecting..." />;
  }

  return <>{children}</>;
}
