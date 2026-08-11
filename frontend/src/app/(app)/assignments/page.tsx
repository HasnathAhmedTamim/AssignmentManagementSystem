"use client";

import { useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { api } from "@/lib/api";
import type { Assignment } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Badge, statusTone } from "@/components/ui/Badge";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { formatDate } from "@/lib/format";

export default function AssignmentsPage() {
  const { user } = useAuth();
  const [items, setItems] = useState<Assignment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setItems(await api.getAssignments());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load assignments");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="Assignments"
        description={
          user?.role === "Student"
            ? "Published assignments for your enrolled classes."
            : user?.role === "Teacher"
              ? "Create, publish, and grade your assignments."
              : "All assignments across the school."
        }
        actions={
          user?.role === "Teacher" ? (
            <Link href="/assignments/new">
              <Button>New assignment</Button>
            </Link>
          ) : undefined
        }
      />
      {error && (
        <div className="mb-4">
          <Alert>{error}</Alert>
        </div>
      )}

      <div className="grid gap-3">
        {items.map((item) => (
          <Link
            key={item.id}
            href={`/assignments/${item.id}`}
            className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm transition hover:border-teal-700/40"
          >
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h2 className="font-display text-lg font-semibold text-slate-900">
                  {item.title}
                </h2>
                <p className="mt-1 text-sm text-slate-600">
                  {item.subjectName} · {item.classRoomName} · {item.teacherName}
                </p>
              </div>
              <Badge tone={statusTone(item.status)}>{item.status}</Badge>
            </div>
            <div className="mt-3 flex flex-wrap gap-4 text-xs text-slate-500">
              <span>Deadline: {formatDate(item.deadline)}</span>
              <span>Max marks: {item.maximumMarks}</span>
              {user?.role === "Student" && item.mySubmission && (
                <span>
                  Your status: {item.mySubmission.status}
                  {item.mySubmission.marks != null
                    ? ` · ${item.mySubmission.marks}`
                    : ""}
                </span>
              )}
            </div>
          </Link>
        ))}
        {!items.length && (
          <div className="rounded-lg border border-dashed border-slate-300 bg-white/60 px-4 py-10 text-center text-sm text-slate-500">
            No assignments to show.
          </div>
        )}
      </div>
    </div>
  );
}
