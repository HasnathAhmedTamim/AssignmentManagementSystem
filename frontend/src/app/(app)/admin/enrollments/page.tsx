"use client";

import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { EnrollmentsManager } from "@/features/admin/enrollments/EnrollmentsManager";

export default function EnrollmentsPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <EnrollmentsManager />
    </ProtectedRoute>
  );
}
