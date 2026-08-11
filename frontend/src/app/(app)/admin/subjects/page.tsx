"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { api, ApiError } from "@/lib/api";
import type { Subject } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Modal } from "@/components/ui/Modal";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { formatDate } from "@/lib/format";

export default function AdminSubjectsPage() {
  return (
    <ProtectedRoute roles={["Admin"]}>
      <SubjectsManager />
    </ProtectedRoute>
  );
}

function SubjectsManager() {
  const [items, setItems] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [open, setOpen] = useState(false);
  const [editing, setEditing] = useState<Subject | null>(null);
  const [name, setName] = useState("");
  const [code, setCode] = useState("");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setItems(await api.getSubjects());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load subjects");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  function openCreate() {
    setEditing(null);
    setName("");
    setCode("");
    setFieldErrors({});
    setOpen(true);
  }

  function openEdit(item: Subject) {
    setEditing(item);
    setName(item.name);
    setCode(item.code);
    setFieldErrors({});
    setOpen(true);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next: Record<string, string> = {};
    if (!name.trim()) next.name = "Name is required";
    if (!code.trim()) next.code = "Code is required";
    setFieldErrors(next);
    if (Object.keys(next).length) return;

    setSaving(true);
    try {
      const payload = { name: name.trim(), code: code.trim() };
      if (editing) await api.updateSubject(editing.id, payload);
      else await api.createSubject(payload);
      setOpen(false);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Save failed");
    } finally {
      setSaving(false);
    }
  }

  async function onDelete(item: Subject) {
    if (!confirm(`Delete subject ${item.name}?`)) return;
    try {
      await api.deleteSubject(item.id);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Delete failed");
    }
  }

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="Subjects"
        description="Manage subject catalog (name and code)."
        actions={<Button onClick={openCreate}>Add subject</Button>}
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
              <th className="px-4 py-3 font-medium">Code</th>
              <th className="px-4 py-3 font-medium">Created</th>
              <th className="px-4 py-3 font-medium">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id} className="border-t border-slate-100">
                <td className="px-4 py-3 font-medium text-slate-900">
                  {item.name}
                </td>
                <td className="px-4 py-3 text-slate-600">{item.code}</td>
                <td className="px-4 py-3 text-slate-600">
                  {formatDate(item.createdAt)}
                </td>
                <td className="px-4 py-3">
                  <div className="flex gap-2">
                    <Button variant="secondary" onClick={() => openEdit(item)}>
                      Edit
                    </Button>
                    <Button variant="danger" onClick={() => onDelete(item)}>
                      Delete
                    </Button>
                  </div>
                </td>
              </tr>
            ))}
            {!items.length && (
              <tr>
                <td colSpan={4} className="px-4 py-8 text-center text-slate-500">
                  No subjects yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal
        open={open}
        title={editing ? "Edit subject" : "Create subject"}
        onClose={() => setOpen(false)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button form="subject-form" type="submit" loading={saving}>
              {editing ? "Save" : "Create"}
            </Button>
          </>
        }
      >
        <form id="subject-form" onSubmit={onSubmit} className="space-y-3" noValidate>
          <Input
            label="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            error={fieldErrors.name}
            placeholder="Mathematics"
          />
          <Input
            label="Code"
            value={code}
            onChange={(e) => setCode(e.target.value)}
            error={fieldErrors.code}
            placeholder="MATH101"
          />
        </form>
      </Modal>
    </div>
  );
}
