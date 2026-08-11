"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { api, ApiError } from "@/lib/api";
import type { TeacherAssignment } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { fromDateTimeLocalValue } from "@/lib/format";

export default function NewAssignmentPage() {
  return (
    <ProtectedRoute roles={["Teacher", "Admin"]}>
      <NewAssignmentForm />
    </ProtectedRoute>
  );
}

function NewAssignmentForm() {
  const router = useRouter();
  const [teacherAssignments, setTeacherAssignments] = useState<
    TeacherAssignment[]
  >([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [form, setForm] = useState({
    teacherClassSubjectId: "",
    title: "",
    description: "",
    deadline: "",
    maximumMarks: "100",
  });
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    let cancelled = false;
    async function load() {
      try {
        const data = await api.getTeacherAssignments();
        if (!cancelled) setTeacherAssignments(data);
      } catch (err) {
        if (!cancelled) {
          setError(
            err instanceof Error ? err.message : "Failed to load teacher assignments"
          );
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }
    load();
    return () => {
      cancelled = true;
    };
  }, []);

  const options = useMemo(
    () =>
      teacherAssignments.map((ta) => ({
        value: ta.id,
        label: `${ta.subjectName} · ${ta.classRoomName} (${ta.classRoomSection})`,
      })),
    [teacherAssignments]
  );

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next: Record<string, string> = {};
    if (!form.teacherClassSubjectId)
      next.teacherClassSubjectId = "Select a class/subject";
    if (!form.title.trim()) next.title = "Title is required";
    if (!form.description.trim()) next.description = "Description is required";
    if (!form.deadline) next.deadline = "Deadline is required";
    const marks = Number(form.maximumMarks);
    if (!form.maximumMarks || Number.isNaN(marks) || marks <= 0)
      next.maximumMarks = "Enter a positive number";
    setFieldErrors(next);
    if (Object.keys(next).length) return;

    setSaving(true);
    setError("");
    try {
      const created = await api.createAssignment({
        teacherClassSubjectId: form.teacherClassSubjectId,
        title: form.title.trim(),
        description: form.description.trim(),
        deadline: fromDateTimeLocalValue(form.deadline),
        maximumMarks: marks,
      });
      router.push(`/assignments/${created.id}`);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Create failed");
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="New assignment"
        description="Creates as Draft. Publish when ready for students."
      />
      {error && (
        <div className="mb-4">
          <Alert>{error}</Alert>
        </div>
      )}
      <form
        onSubmit={onSubmit}
        className="max-w-2xl space-y-4 rounded-lg border border-slate-200 bg-white p-5 shadow-sm"
        noValidate
      >
        <Select
          label="Class / Subject"
          value={form.teacherClassSubjectId}
          onChange={(e) =>
            setForm({ ...form, teacherClassSubjectId: e.target.value })
          }
          options={options}
          placeholder="Select teaching assignment"
          error={fieldErrors.teacherClassSubjectId}
        />
        <Input
          label="Title"
          value={form.title}
          onChange={(e) => setForm({ ...form, title: e.target.value })}
          error={fieldErrors.title}
        />
        <Textarea
          label="Description"
          rows={5}
          value={form.description}
          onChange={(e) => setForm({ ...form, description: e.target.value })}
          error={fieldErrors.description}
        />
        <Input
          label="Deadline"
          type="datetime-local"
          value={form.deadline}
          onChange={(e) => setForm({ ...form, deadline: e.target.value })}
          error={fieldErrors.deadline}
        />
        <Input
          label="Maximum marks"
          type="number"
          min={1}
          value={form.maximumMarks}
          onChange={(e) => setForm({ ...form, maximumMarks: e.target.value })}
          error={fieldErrors.maximumMarks}
        />
        <div className="flex gap-2">
          <Button type="submit" loading={saving}>
            Create draft
          </Button>
          <Button
            type="button"
            variant="secondary"
            onClick={() => router.push("/assignments")}
          >
            Cancel
          </Button>
        </div>
      </form>
    </div>
  );
}
