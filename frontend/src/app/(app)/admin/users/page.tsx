"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { api, ApiError } from "@/lib/api";
import type { Role, User } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { Badge, statusTone } from "@/components/ui/Badge";

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

export default function AdminUsersPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <UsersManager />
    </ProtectedRoute>
  );
}

function UsersManager() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<User | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setUsers(await api.getUsers());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load users");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setFieldErrors({});
    setOpen(true);
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
    setFieldErrors({});
    setOpen(true);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next: Record<string, string> = {};
    if (!form.fullName.trim()) next.fullName = "Full name is required";
    if (!form.email.trim()) next.email = "Email is required";
    else if (!/^\S+@\S+\.\S+$/.test(form.email)) next.email = "Invalid email";
    if (!editing && !form.password) next.password = "Password is required";
    else if (!editing && form.password.length < 6)
      next.password = "At least 6 characters";
    setFieldErrors(next);
    if (Object.keys(next).length) return;

    setSaving(true);
    setError("");
    try {
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
      setOpen(false);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Save failed");
    } finally {
      setSaving(false);
    }
  }

  async function onDelete(user: User) {
    if (!confirm(`Delete ${user.fullName}?`)) return;
    try {
      await api.deleteUser(user.id);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Delete failed");
    }
  }

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="Users"
        description="Create and manage admin, teacher, and student accounts."
        actions={<Button onClick={openCreate}>Add user</Button>}
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
              <th className="px-4 py-3 font-medium">Name</th>
              <th className="px-4 py-3 font-medium">Email</th>
              <th className="px-4 py-3 font-medium">Role</th>
              <th className="px-4 py-3 font-medium">Status</th>
              <th className="px-4 py-3 font-medium">Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id} className="border-t border-slate-100">
                <td className="px-4 py-3 font-medium text-slate-900">
                  {u.fullName}
                </td>
                <td className="px-4 py-3 text-slate-600">{u.email}</td>
                <td className="px-4 py-3">
                  <Badge tone={statusTone(u.role)}>{u.role}</Badge>
                </td>
                <td className="px-4 py-3">
                  <Badge tone={u.isActive ? "teal" : "slate"}>
                    {u.isActive ? "Active" : "Inactive"}
                  </Badge>
                </td>
                <td className="px-4 py-3">
                  <div className="flex gap-2">
                    <Button variant="secondary" onClick={() => openEdit(u)}>
                      Edit
                    </Button>
                    <Button variant="danger" onClick={() => onDelete(u)}>
                      Delete
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
            {!users.length && (
              <tr>
                <td colSpan={5} className="px-4 py-8 text-center text-slate-500">
                  No users yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal
        open={open}
        title={editing ? "Edit user" : "Create user"}
        onClose={() => setOpen(false)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button form="user-form" type="submit" loading={saving}>
              {editing ? "Save" : "Create"}
            </Button>
          </>
        }
      >
        <form id="user-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Input
            label="Full name"
            value={form.fullName}
            onChange={(e) => setForm({ ...form, fullName: e.target.value })}
            error={fieldErrors.fullName}
          />
          <Input
            label="Email"
            type="email"
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            error={fieldErrors.email}
          />
          {!editing && (
            <Input
              label="Password"
              type="password"
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              error={fieldErrors.password}
            />
          )}
          <Select
            label="Role"
            value={form.role}
            onChange={(e) =>
              setForm({ ...form, role: e.target.value as Role })
            }
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
      </Modal>
    </div>
  );
}
