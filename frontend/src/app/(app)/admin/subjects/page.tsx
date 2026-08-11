"use client";

import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { SubjectsManager } from "@/features/admin/subjects/SubjectsManager";

export default function AdminSubjectsPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <SubjectsManager />
    </ProtectedRoute>
  );
}
