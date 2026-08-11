"use client";

import { FormEvent, useCallback, useMemo, useState } from "react";
import { api } from "@/lib/api";
import type { ClassRoom, Subject, TeacherAssignment, User } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { formatDate } from "@/lib/format";
import { useAsyncList } from "@/shared/hooks/useAsyncList";
import { useCrudModal } from "@/shared/hooks/useCrudModal";
import { validateFields } from "@/shared/lib/validation";
import { AdminPageShell } from "@/shared/components/AdminPageShell";
import { DataTable } from "@/shared/components/DataTable";
import { FormModal } from "@/shared/components/FormModal";

interface TeacherAssignmentPageData {
  items: TeacherAssignment[];
  teachers: User[];
  classes: ClassRoom[];
  subjects: Subject[];
}

export function TeacherAssignmentsManager() {
  const loader = useCallback(async (): Promise<TeacherAssignmentPageData> => {
    const [ta, users, rooms, subs] = await Promise.all([
      api.getTeacherAssignments(),
      api.getUsers(),
      api.getClassRooms(),
      api.getSubjects(),
    ]);
    return {
      items: ta,
      teachers: users.filter((u) => u.role === "Teacher" && u.isActive),
      classes: rooms,
      subjects: subs,
    };
  }, []);

  const { data, loading, error, reload } = useAsyncList({ loader });
  const items = data?.items ?? [];

  const crud = useCrudModal({ onSaved: reload });
  const [teacherId, setTeacherId] = useState("");
  const [classRoomId, setClassRoomId] = useState("");
  const [subjectId, setSubjectId] = useState("");

  const teacherOptions = useMemo(
    () =>
      (data?.teachers ?? []).map((t) => ({
        value: t.id,
        label: `${t.fullName} (${t.email})`,
      })),
    [data?.teachers]
  );
  const classOptions = useMemo(
    () =>
      (data?.classes ?? []).map((c) => ({
        value: c.id,
        label: `${c.name} — ${c.section}`,
      })),
    [data?.classes]
  );
  const subjectOptions = useMemo(
    () =>
      (data?.subjects ?? []).map((s) => ({
        value: s.id,
        label: `${s.name} (${s.code})`,
      })),
    [data?.subjects]
  );

  function openCreate() {
    setTeacherId("");
    setClassRoomId("");
    setSubjectId("");
    crud.openModal();
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next = validateFields({
      teacherId: { value: teacherId, message: "Select a teacher" },
      classRoomId: { value: classRoomId, message: "Select a class" },
      subjectId: { value: subjectId, message: "Select a subject" },
    });
    crud.setFieldErrors(next);
    if (Object.keys(next).length) return;

    await crud.runSave(() =>
      api.createTeacherAssignment({ teacherId, classRoomId, subjectId })
    );
  }

  return (
    <AdminPageShell
      title="Teacher Assignments"
      description="Assign teachers to a class and subject."
      actions={<Button onClick={openCreate}>Assign teacher</Button>}
      loading={loading}
      error={error || crud.error}
    >
      <DataTable
        rows={items}
        rowKey={(item) => item.id}
        emptyMessage="No teacher assignments yet."
        columns={[
          {
            key: "teacher",
            header: "Teacher",
            className: "px-4 py-3 font-medium text-slate-900",
            cell: (item) => item.teacherName,
          },
          {
            key: "class",
            header: "Class",
            cell: (item) => `${item.classRoomName} (${item.classRoomSection})`,
          },
          {
            key: "subject",
            header: "Subject",
            cell: (item) => `${item.subjectName} (${item.subjectCode})`,
          },
          {
            key: "created",
            header: "Created",
            cell: (item) => formatDate(item.createdAt),
          },
          {
            key: "actions",
            header: "Actions",
            cell: (item) => (
              <Button
                variant="danger"
                onClick={() =>
                  crud.runDelete(
                    `Remove ${item.teacherName} from ${item.subjectName}?`,
                    () => api.deleteTeacherAssignment(item.id)
                  )
                }
              >
                Delete
              </Button>
            ),
          },
        ]}
      />

      <FormModal
        open={crud.open}
        title="Assign teacher"
        formId="ta-form"
        saving={crud.saving}
        submitLabel="Assign"
        onClose={crud.close}
      >
        <form id="ta-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Select
            label="Teacher"
            value={teacherId}
            onChange={(e) => setTeacherId(e.target.value)}
            options={teacherOptions}
            placeholder="Select teacher"
            error={crud.fieldErrors.teacherId}
          />
          <Select
            label="Class"
            value={classRoomId}
            onChange={(e) => setClassRoomId(e.target.value)}
            options={classOptions}
            placeholder="Select class"
            error={crud.fieldErrors.classRoomId}
          />
          <Select
            label="Subject"
            value={subjectId}
            onChange={(e) => setSubjectId(e.target.value)}
            options={subjectOptions}
            placeholder="Select subject"
            error={crud.fieldErrors.subjectId}
          />
        </form>
      </FormModal>
    </AdminPageShell>
  );
}
