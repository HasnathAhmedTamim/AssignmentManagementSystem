"use client";

import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { TeacherAssignmentsManager } from "@/features/admin/teacher-assignments/TeacherAssignmentsManager";

export default function TeacherAssignmentsPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <TeacherAssignmentsManager />
    </ProtectedRoute>
  );
}
