"use client";

import { FormEvent, useCallback, useState } from "react";
import { api } from "@/lib/api";
import type { Role, User } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Badge, statusTone } from "@/components/ui/Badge";
import { useAsyncList } from "@/shared/hooks/useAsyncList";
import { useCrudModal } from "@/shared/hooks/useCrudModal";
import { isValidEmail, validateFields } from "@/shared/lib/validation";
import { AdminPageShell } from "@/shared/components/AdminPageShell";
import { DataTable } from "@/shared/components/DataTable";
import { FormModal } from "@/shared/components/FormModal";

const roleOptions = [
  { value: "Admin", label: "Admin" },
  { value: "Teacher", label: "Teacher" },
  { value: "Student", label: "Student" },
];

const emptyForm = {
  fullName: "",
  email: "",
  password: "",
  role: "Student" as Role,
  isActive: true,
};

export function UsersManager() {
  const loader = useCallback(() => api.getUsers(), []);
  const { data, loading, error, reload } = useAsyncList({ loader });
  const users = data ?? [];

  const crud = useCrudModal({ onSaved: reload });
  const [editing, setEditing] = useState<User | null>(null);
  const [form, setForm] = useState(emptyForm);

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    crud.openModal();
  }

  function openEdit(user: User) {
    setEditing(user);
    setForm({
      fullName: user.fullName,
      email: user.email,
      password: "",
      role: user.role,
      isActive: user.isActive,
    });
    crud.openModal();
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next = validateFields({
      fullName: { value: form.fullName, message: "Full name is required" },
      email: {
        value: form.email,
        message: "Email is required",
        test: (v) => Boolean(v.trim()) && isValidEmail(v),
      },
    });

    if (!editing) {
      if (!form.password) next.password = "Password is required";
      else if (form.password.length < 6) next.password = "At least 6 characters";
    }
    if (form.email.trim() && !isValidEmail(form.email)) {
      next.email = "Invalid email";
    }

    crud.setFieldErrors(next);
    if (Object.keys(next).length) return;

    await crud.runSave(async () => {
      if (editing) {
        await api.updateUser(editing.id, {
          fullName: form.fullName.trim(),
          email: form.email.trim(),
          role: form.role,
          isActive: form.isActive,
        });
      } else {
        await api.createUser({
          fullName: form.fullName.trim(),
          email: form.email.trim(),
          password: form.password,
          role: form.role,
        });
      }
    });
  }

  return (
    <AdminPageShell
      title="Users"
      description="Create and manage admin, teacher, and student accounts."
      actions={<Button onClick={openCreate}>Add user</Button>}
      loading={loading}
      error={error || crud.error}
    >
      <DataTable
        rows={users}
        rowKey={(u) => u.id}
        emptyMessage="No users yet."
        columns={[
          {
            key: "name",
            header: "Name",
            className: "px-4 py-3 font-medium text-slate-900",
            cell: (u) => u.fullName,
          },
          { key: "email", header: "Email", cell: (u) => u.email },
          {
            key: "role",
            header: "Role",
            cell: (u) => <Badge tone={statusTone(u.role)}>{u.role}</Badge>,
          },
          {
            key: "status",
            header: "Status",
            cell: (u) => (
              <Badge tone={u.isActive ? "teal" : "slate"}>
                {u.isActive ? "Active" : "Inactive"}
              </Badge>
            ),
          },
          {
            key: "actions",
            header: "Actions",
            cell: (u) => (
              <div className="flex gap-2">
                <Button variant="secondary" onClick={() => openEdit(u)}>
                  Edit
                </Button>
                <Button
                  variant="danger"
                  onClick={() =>
                    crud.runDelete(`Delete ${u.fullName}?`, () =>
                      api.deleteUser(u.id)
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
        title={editing ? "Edit user" : "Create user"}
        formId="user-form"
        saving={crud.saving}
        submitLabel={editing ? "Save" : "Create"}
        onClose={crud.close}
      >
        <form id="user-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Input
            label="Full name"
            value={form.fullName}
            onChange={(e) => setForm({ ...form, fullName: e.target.value })}
            error={crud.fieldErrors.fullName}
          />
          <Input
            label="Email"
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            error={crud.fieldErrors.email}
          />
          {!editing && (
            <Input
              label="Password"
              type="password"
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              error={crud.fieldErrors.password}
            />
          )}
          <Select
            label="Role"
            value={form.role}
            onChange={(e) => setForm({ ...form, role: e.target.value as Role })}
            options={roleOptions}
          />
          {editing && (
            <label className="flex items-center gap-2 text-sm text-slate-700">
              <input
                type="checkbox"
                checked={form.isActive}
                onChange={(e) =>
                  setForm({ ...form, isActive: e.target.checked })
                }
              />
              Active
            </label>
          )}
        </form>
      </FormModal>
    </AdminPageShell>
  );
}
