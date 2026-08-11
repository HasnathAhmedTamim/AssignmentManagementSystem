"use client";

import { FormEvent, useCallback, useState } from "react";
import { api } from "@/lib/api";
import type { Subject } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { formatDate } from "@/lib/format";
import { useAsyncList } from "@/shared/hooks/useAsyncList";
import { useCrudModal } from "@/shared/hooks/useCrudModal";
import { validateFields } from "@/shared/lib/validation";
import { AdminPageShell } from "@/shared/components/AdminPageShell";
import { DataTable } from "@/shared/components/DataTable";
import { FormModal } from "@/shared/components/FormModal";

export function SubjectsManager() {
  const loader = useCallback(() => api.getSubjects(), []);
  const { data, loading, error, reload } = useAsyncList({ loader });
  const items = data ?? [];

  const crud = useCrudModal({ onSaved: reload });
  const [editing, setEditing] = useState<Subject | null>(null);
  const [name, setName] = useState("");
  const [code, setCode] = useState("");

  function openCreate() {
    setEditing(null);
    setName("");
    setCode("");
    crud.openModal();
  }

  function openEdit(item: Subject) {
    setEditing(item);
    setName(item.name);
    setCode(item.code);
    crud.openModal();
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next = validateFields({
      name: { value: name, message: "Name is required" },
      code: { value: code, message: "Code is required" },
    });
    crud.setFieldErrors(next);
    if (Object.keys(next).length) return;

    const payload = { name: name.trim(), code: code.trim() };
    await crud.runSave(async () => {
      if (editing) await api.updateSubject(editing.id, payload);
      else await api.createSubject(payload);
    });
  }

  return (
    <AdminPageShell
      title="Subjects"
      description="Manage subject catalog (name and code)."
      actions={<Button onClick={openCreate}>Add subject</Button>}
      loading={loading}
      error={error || crud.error}
    >
      <DataTable
        rows={items}
        rowKey={(item) => item.id}
        emptyMessage="No subjects yet."
        columns={[
          {
            key: "name",
            header: "Name",
            className: "px-4 py-3 font-medium text-slate-900",
            cell: (item) => item.name,
          },
          { key: "code", header: "Code", cell: (item) => item.code },
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
                    crud.runDelete(`Delete subject ${item.name}?`, () =>
                      api.deleteSubject(item.id)
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
        title={editing ? "Edit subject" : "Create subject"}
        formId="subject-form"
        saving={crud.saving}
        submitLabel={editing ? "Save" : "Create"}
        onClose={crud.close}
      >
        <form id="subject-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Input
            label="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            error={crud.fieldErrors.name}
            placeholder="Mathematics"
          />
          <Input
            label="Code"
            value={code}
            onChange={(e) => setCode(e.target.value)}
            error={crud.fieldErrors.code}
            placeholder="MATH101"
          />
        </form>
      </FormModal>
    </AdminPageShell>
  );
}
