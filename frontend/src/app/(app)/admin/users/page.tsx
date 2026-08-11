"use client";

import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { UsersManager } from "@/features/admin/users/UsersManager";

export default function AdminUsersPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <UsersManager />
    </ProtectedRoute>
  );
}
