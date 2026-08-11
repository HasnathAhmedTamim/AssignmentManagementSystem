"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { api, ApiError } from "@/lib/api";
import type { ClassRoom, Enrollment, User } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { formatDate } from "@/lib/format";

export default function EnrollmentsPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <EnrollmentsManager />
    </ProtectedRoute>
  );
}

function EnrollmentsManager() {
  const [items, setItems] = useState<Enrollment[]>([]);
  const [students, setStudents] = useState<User[]>([]);
  const [classes, setClasses] = useState<ClassRoom[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [open, setOpen] = useState(false);
  const [studentId, setStudentId] = useState("");
  const [classRoomId, setClassRoomId] = useState("");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const [enrollments, users, rooms] = await Promise.all([
        api.getEnrollments(),
        api.getUsers(),
        api.getClassRooms(),
      ]);
      setItems(enrollments);
      setStudents(users.filter((u) => u.role === "Student" && u.isActive));
      setClasses(rooms);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load data");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const studentOptions = useMemo(
    () =>
      students.map((s) => ({
        value: s.id,
        label: `${s.fullName} (${s.email})`,
      })),
    [students]
  );
  const classOptions = useMemo(
    () =>
      classes.map((c) => ({
        value: c.id,
        label: `${c.name} — ${c.section}`,
      })),
    [classes]
  );

  function openCreate() {
    setStudentId("");
    setClassRoomId("");
    setFieldErrors({});
    setOpen(true);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next: Record<string, string> = {};
    if (!studentId) next.studentId = "Select a student";
    if (!classRoomId) next.classRoomId = "Select a class";
    setFieldErrors(next);
    if (Object.keys(next).length) return;

    setSaving(true);
    try {
      await api.createEnrollment({ studentId, classRoomId });
      setOpen(false);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Create failed");
    } finally {
      setSaving(false);
    }
  }

  async function onDelete(item: Enrollment) {
    if (
      !confirm(
        `Remove enrollment for ${item.studentName} from ${item.classRoomName}?`
      )
    )
      return;
    try {
      await api.deleteEnrollment(item.id);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Delete failed");
    }
  }

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="Enrollments"
        description="Enroll students into classrooms."
        actions={<Button onClick={openCreate}>Enroll student</Button>}
      />
      {error && (
        <div className="mb-4">
          <Alert>{error}</Alert>
        </div>
      )}
      <div className="overflow-x-auto rounded-lg border border-slate-200 bg-white shadow-sm">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-slate-50 text-slate-600">
            <tr>
              <th className="px-4 py-3 font-medium">Student</th>
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Enrolled</th>
              <th className="px-4 py-3 font-medium">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id} className="border-t border-slate-100">
                <td className="px-4 py-3 font-medium text-slate-900">
                  {item.studentName}
                </td>
                <td className="px-4 py-3 text-slate-600">
                  {item.classRoomName} ({item.classRoomSection})
                </td>
                <td className="px-4 py-3 text-slate-600">
                  {formatDate(item.createdAt)}
                </td>
                <td className="px-4 py-3">
                  <Button variant="danger" onClick={() => onDelete(item)}>
                    Delete
                  </Button>
                </td>
              </tr>
            ))}
            {!items.length && (
              <tr>
                <td colSpan={4} className="px-4 py-8 text-center text-slate-500">
                  No enrollments yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal
        open={open}
        title="Enroll student"
        onClose={() => setOpen(false)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button form="enroll-form" type="submit" loading={saving}>
              Enroll
            </Button>
          </>
        }
      >
        <form id="enroll-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Select
            label="Student"
            value={studentId}
            onChange={(e) => setStudentId(e.target.value)}
            options={studentOptions}
            placeholder="Select student"
            error={fieldErrors.studentId}
          />
          <Select
            label="Class"
            value={classRoomId}
            onChange={(e) => setClassRoomId(e.target.value)}
            options={classOptions}
            placeholder="Select class"
            error={fieldErrors.classRoomId}
          />
        </form>
      </Modal>
    </div>
  );
}
