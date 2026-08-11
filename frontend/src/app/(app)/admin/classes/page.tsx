"use client";

import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { ClassesManager } from "@/features/admin/classes/ClassesManager";

export default function AdminClassesPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <ClassesManager />
    </ProtectedRoute>
  );
}
