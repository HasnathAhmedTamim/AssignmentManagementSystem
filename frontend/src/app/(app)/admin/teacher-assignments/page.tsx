"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { api, ApiError } from "@/lib/api";
import type { ClassRoom, Subject, TeacherAssignment, User } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { formatDate } from "@/lib/format";

export default function TeacherAssignmentsPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <TeacherAssignmentsManager />
    </ProtectedRoute>
  );
}

function TeacherAssignmentsManager() {
  const [items, setItems] = useState<TeacherAssignment[]>([]);
  const [teachers, setTeachers] = useState<User[]>([]);
  const [classes, setClasses] = useState<ClassRoom[]>([]);
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [open, setOpen] = useState(false);
  const [teacherId, setTeacherId] = useState("");
  const [classRoomId, setClassRoomId] = useState("");
  const [subjectId, setSubjectId] = useState("");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const [ta, users, rooms, subs] = await Promise.all([
        api.getTeacherAssignments(),
        api.getUsers(),
        api.getClassRooms(),
        api.getSubjects(),
      ]);
      setItems(ta);
      setTeachers(users.filter((u) => u.role === "Teacher" && u.isActive));
      setClasses(rooms);
      setSubjects(subs);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load data");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const teacherOptions = useMemo(
    () => teachers.map((t) => ({ value: t.id, label: `${t.fullName} (${t.email})` })),
    [teachers]
  );
  const classOptions = useMemo(
    () =>
      classes.map((c) => ({
        value: c.id,
        label: `${c.name} — ${c.section}`,
      })),
    [classes]
  );
  const subjectOptions = useMemo(
    () => subjects.map((s) => ({ value: s.id, label: `${s.name} (${s.code})` })),
    [subjects]
  );

  function openCreate() {
    setTeacherId("");
    setClassRoomId("");
    setSubjectId("");
    setFieldErrors({});
    setOpen(true);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next: Record<string, string> = {};
    if (!teacherId) next.teacherId = "Select a teacher";
    if (!classRoomId) next.classRoomId = "Select a class";
    if (!subjectId) next.subjectId = "Select a subject";
    setFieldErrors(next);
    if (Object.keys(next).length) return;

    setSaving(true);
    try {
      await api.createTeacherAssignment({ teacherId, classRoomId, subjectId });
      setOpen(false);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Create failed");
    } finally {
      setSaving(false);
    }
  }

  async function onDelete(item: TeacherAssignment) {
    if (
      !confirm(
        `Remove assignment for ${item.teacherName} — ${item.subjectName} / ${item.classRoomName}?`
      )
    )
      return;
    try {
      await api.deleteTeacherAssignment(item.id);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Delete failed");
    }
  }

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="Teacher assignments"
        description="Link teachers to a class and subject."
        actions={<Button onClick={openCreate}>Assign teacher</Button>}
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
              <th className="px-4 py-3 font-medium">Teacher</th>
              <th className="px-4 py-3 font-medium">Class</th>
              <th className="px-4 py-3 font-medium">Subject</th>
              <th className="px-4 py-3 font-medium">Created</th>
              <th className="px-4 py-3 font-medium">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id} className="border-t border-slate-100">
                <td className="px-4 py-3 font-medium text-slate-900">
                  {item.teacherName}
                </td>
                <td className="px-4 py-3 text-slate-600">
                  {item.classRoomName} ({item.classRoomSection})
                </td>
                <td className="px-4 py-3 text-slate-600">
                  {item.subjectName} ({item.subjectCode})
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
                <td colSpan={5} className="px-4 py-8 text-center text-slate-500">
                  No teacher assignments yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal
        open={open}
        title="Assign teacher"
        onClose={() => setOpen(false)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button form="ta-form" type="submit" loading={saving}>
              Create
            </Button>
          </>
        }
      >
        <form id="ta-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Select
            label="Teacher"
            value={teacherId}
            onChange={(e) => setTeacherId(e.target.value)}
            options={teacherOptions}
            placeholder="Select teacher"
            error={fieldErrors.teacherId}
          />
          <Select
            label="Class"
            value={classRoomId}
            onChange={(e) => setClassRoomId(e.target.value)}
            options={classOptions}
            placeholder="Select class"
            error={fieldErrors.classRoomId}
          />
          <Select
            label="Subject"
            value={subjectId}
            onChange={(e) => setSubjectId(e.target.value)}
            options={subjectOptions}
            placeholder="Select subject"
            error={fieldErrors.subjectId}
          />
        </form>
      </Modal>
    </div>
  );
}
