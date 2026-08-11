"use client";

import { FormEvent, useCallback, useMemo, useState } from "react";
import { api } from "@/lib/api";
import type { ClassRoom, Enrollment, User } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { formatDate } from "@/lib/format";
import { useAsyncList } from "@/shared/hooks/useAsyncList";
import { useCrudModal } from "@/shared/hooks/useCrudModal";
import { validateFields } from "@/shared/lib/validation";
import { AdminPageShell } from "@/shared/components/AdminPageShell";
import { DataTable } from "@/shared/components/DataTable";
import { FormModal } from "@/shared/components/FormModal";

interface EnrollmentPageData {
  items: Enrollment[];
  students: User[];
  classes: ClassRoom[];
}

export function EnrollmentsManager() {
  const loader = useCallback(async (): Promise<EnrollmentPageData> => {
    const [enrollments, users, rooms] = await Promise.all([
      api.getEnrollments(),
      api.getUsers(),
      api.getClassRooms(),
    ]);
    return {
      items: enrollments,
      students: users.filter((u) => u.role === "Student" && u.isActive),
      classes: rooms,
    };
  }, []);

  const { data, loading, error, reload } = useAsyncList({ loader });
  const items = data?.items ?? [];

  const crud = useCrudModal({ onSaved: reload });
  const [studentId, setStudentId] = useState("");
  const [classRoomId, setClassRoomId] = useState("");

  const studentOptions = useMemo(
    () =>
      (data?.students ?? []).map((s) => ({
        value: s.id,
        label: `${s.fullName} (${s.email})`,
      })),
    [data?.students]
  );
  const classOptions = useMemo(
    () =>
      (data?.classes ?? []).map((c) => ({
        value: c.id,
        label: `${c.name} — ${c.section}`,
      })),
    [data?.classes]
  );

  function openCreate() {
    setStudentId("");
    setClassRoomId("");
    crud.openModal();
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next = validateFields({
      studentId: { value: studentId, message: "Select a student" },
      classRoomId: { value: classRoomId, message: "Select a class" },
    });
    crud.setFieldErrors(next);
    if (Object.keys(next).length) return;

    await crud.runSave(() =>
      api.createEnrollment({ studentId, classRoomId })
    );
  }

  return (
    <AdminPageShell
      title="Enrollments"
      description="Enroll students into classrooms."
      actions={<Button onClick={openCreate}>Enroll student</Button>}
      loading={loading}
      error={error || crud.error}
    >
      <DataTable
        rows={items}
        rowKey={(item) => item.id}
        emptyMessage="No enrollments yet."
        columns={[
          {
            key: "student",
            header: "Student",
            className: "px-4 py-3 font-medium text-slate-900",
            cell: (item) => item.studentName,
          },
          {
            key: "class",
            header: "Class",
            cell: (item) => `${item.classRoomName} (${item.classRoomSection})`,
          },
          {
            key: "enrolled",
            header: "Enrolled",
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
                    `Remove enrollment for ${item.studentName} from ${item.classRoomName}?`,
                    () => api.deleteEnrollment(item.id)
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
        title="Enroll student"
        formId="enroll-form"
        saving={crud.saving}
        submitLabel="Enroll"
        onClose={crud.close}
      >
        <form id="enroll-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Select
            label="Student"
            value={studentId}
            onChange={(e) => setStudentId(e.target.value)}
            options={studentOptions}
            placeholder="Select student"
            error={crud.fieldErrors.studentId}
          />
          <Select
            label="Class"
            value={classRoomId}
            onChange={(e) => setClassRoomId(e.target.value)}
            options={classOptions}
            placeholder="Select class"
            error={crud.fieldErrors.classRoomId}
          />
        </form>
      </FormModal>
    </AdminPageShell>
  );
}
