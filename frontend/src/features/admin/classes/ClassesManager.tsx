"use client";

import { FormEvent, useCallback, useState } from "react";
import { api } from "@/lib/api";
import type { ClassRoom } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { formatDate } from "@/lib/format";
import { useAsyncList } from "@/shared/hooks/useAsyncList";
import { useCrudModal } from "@/shared/hooks/useCrudModal";
import { validateFields } from "@/shared/lib/validation";
import { AdminPageShell } from "@/shared/components/AdminPageShell";
import { DataTable } from "@/shared/components/DataTable";
import { FormModal } from "@/shared/components/FormModal";

export function ClassesManager() {
  const loader = useCallback(() => api.getClassRooms(), []);
  const { data, loading, error, reload } = useAsyncList({ loader });
  const items = data ?? [];

  const crud = useCrudModal({ onSaved: reload });
  const [editing, setEditing] = useState<ClassRoom | null>(null);
  const [name, setName] = useState("");
  const [section, setSection] = useState("");

  function openCreate() {
    setEditing(null);
    setName("");
    setSection("");
    crud.openModal();
  }

  function openEdit(item: ClassRoom) {
    setEditing(item);
    setName(item.name);
    setSection(item.section);
    crud.openModal();
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next = validateFields({
      name: { value: name, message: "Name is required" },
      section: { value: section, message: "Section is required" },
    });
    crud.setFieldErrors(next);
    if (Object.keys(next).length) return;

    const payload = { name: name.trim(), section: section.trim() };
    await crud.runSave(async () => {
      if (editing) await api.updateClassRoom(editing.id, payload);
      else await api.createClassRoom(payload);
    });
  }

  return (
    <AdminPageShell
      title="Classes"
      description="Manage classrooms and sections."
      actions={<Button onClick={openCreate}>Add class</Button>}
      loading={loading}
      error={error || crud.error}
    >
      <DataTable
        rows={items}
        rowKey={(item) => item.id}
        emptyMessage="No classes yet."
        columns={[
          {
            key: "name",
            header: "Name",
            className: "px-4 py-3 font-medium text-slate-900",
            cell: (item) => item.name,
          },
          { key: "section", header: "Section", cell: (item) => item.section },
          {
            key: "created",
            header: "Created",
            cell: (item) => formatDate(item.createdAt),
          },
          {
            key: "actions",
            header: "Actions",
            cell: (item) => (
              <div className="flex gap-2">
                <Button variant="secondary" onClick={() => openEdit(item)}>
                  Edit
                </Button>
                <Button
                  variant="danger"
                  onClick={() =>
                    crud.runDelete(
                      `Delete class ${item.name} (${item.section})?`,
                      () => api.deleteClassRoom(item.id)
                    )
                  }
                >
                  Delete
                </Button>
              </div>
            ),
          },
        ]}
      />

      <FormModal
        open={crud.open}
        title={editing ? "Edit class" : "Create class"}
        formId="class-form"
        saving={crud.saving}
        submitLabel={editing ? "Save" : "Create"}
        onClose={crud.close}
      >
        <form id="class-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Input
            label="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            error={crud.fieldErrors.name}
            placeholder="Grade 10"
          />
          <Input
            label="Section"
            value={section}
            onChange={(e) => setSection(e.target.value)}
            error={crud.fieldErrors.section}
            placeholder="A"
          />
        </form>
      </FormModal>
    </AdminPageShell>
  );
}
