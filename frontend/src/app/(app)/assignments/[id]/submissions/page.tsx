"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { useParams } from "next/navigation";
import { ProtectedRoute } from "@/components/layout/ProtectedRoute";
import { api, ApiError } from "@/lib/api";
import type { Assignment, Submission, SubmissionStatus } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { Modal } from "@/components/ui/Modal";
import { Badge, statusTone } from "@/components/ui/Badge";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import { formatDate } from "@/lib/format";

const statusOptions = [
  { value: "Pending", label: "Pending" },
  { value: "Reviewed", label: "Reviewed" },
  { value: "Late", label: "Late" },
];

export default function AssignmentSubmissionsPage() {
  return (
    <ProtectedRoute roles={["Teacher", "Admin"]}>
      <SubmissionsManager />
    </ProtectedRoute>
  );
}

function SubmissionsManager() {
  const params = useParams<{ id: string }>();
  const assignmentId = params.id;
  const [assignment, setAssignment] = useState<Assignment | null>(null);
  const [items, setItems] = useState<Submission[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [grading, setGrading] = useState<Submission | null>(null);
  const [marks, setMarks] = useState("");
  const [feedback, setFeedback] = useState("");
  const [status, setStatus] = useState<SubmissionStatus>("Reviewed");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const [asg, subs] = await Promise.all([
        api.getAssignment(assignmentId),
        api.getSubmissionsByAssignment(assignmentId),
      ]);
      setAssignment(asg);
      setItems(subs);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load submissions");
    } finally {
      setLoading(false);
    }
  }, [assignmentId]);

  useEffect(() => {
    load();
  }, [load]);

  function openGrade(sub: Submission) {
    setGrading(sub);
    setMarks(sub.marks != null ? String(sub.marks) : "");
    setFeedback(sub.feedback || "");
    setStatus(sub.status === "Pending" ? "Reviewed" : sub.status);
    setFieldErrors({});
  }

  async function onGrade(e: FormEvent) {
    e.preventDefault();
    if (!grading || !assignment) return;
    const next: Record<string, string> = {};
    const value = Number(marks);
    if (marks === "" || Number.isNaN(value) || value < 0)
      next.marks = "Enter valid marks";
    else if (value > assignment.maximumMarks)
      next.marks = `Max is ${assignment.maximumMarks}`;
    setFieldErrors(next);
    if (Object.keys(next).length) return;

    setSaving(true);
    try {
      await api.gradeSubmission(grading.id, {
        marks: value,
        feedback: feedback.trim() || null,
        status,
      });
      setGrading(null);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Grading failed");
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader
        title="Submissions"
        description={
          assignment
            ? `${assignment.title} · max ${assignment.maximumMarks} marks`
            : "Grade student submissions"
        }
        actions={
          <Link href={`/assignments/${assignmentId}`}>
            <Button variant="secondary">Back to assignment</Button>
          </Link>
        }
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
              <th className="px-4 py-3 font-medium">Student</th>
              <th className="px-4 py-3 font-medium">Submitted</th>
              <th className="px-4 py-3 font-medium">Status</th>
              <th className="px-4 py-3 font-medium">Marks</th>
              <th className="px-4 py-3 font-medium">Actions</th>
            </tr>
          </thead>
          <tbody>
            {items.map((item) => (
              <tr key={item.id} className="border-t border-slate-100 align-top">
                <td className="px-4 py-3">
                  <p className="font-medium text-slate-900">{item.studentName}</p>
                  <p className="mt-1 max-w-md whitespace-pre-wrap text-xs text-slate-500 line-clamp-3">
                    {item.answer}
                  </p>
                </td>
                <td className="px-4 py-3 text-slate-600">
                  {formatDate(item.submittedAt)}
                </td>
                <td className="px-4 py-3">
                  <Badge tone={statusTone(item.status)}>{item.status}</Badge>
                </td>
                <td className="px-4 py-3 text-slate-700">
                  {item.marks != null ? item.marks : "—"}
                </td>
                <td className="px-4 py-3">
                  <Button variant="secondary" onClick={() => openGrade(item)}>
                    Grade
                  </Button>
                </td>
              </tr>
            ))}
            {!items.length && (
              <tr>
                <td colSpan={5} className="px-4 py-8 text-center text-slate-500">
                  No submissions yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal
        open={!!grading}
        title={grading ? `Grade ${grading.studentName}` : "Grade"}
        onClose={() => setGrading(null)}
        footer={
          <>
            <Button variant="secondary" onClick={() => setGrading(null)}>
              Cancel
            </Button>
            <Button form="grade-form" type="submit" loading={saving}>
              Save grade
            </Button>
          </>
        }
      >
        {grading && (
          <form id="grade-form" onSubmit={onGrade} className="space-y-3" noValidate>
            <div className="rounded-md bg-slate-50 p-3 text-sm text-slate-700">
              <p className="font-medium text-slate-500">Answer</p>
              <p className="mt-1 whitespace-pre-wrap">{grading.answer}</p>
            </div>
            <Input
              label={`Marks (max ${assignment?.maximumMarks ?? "—"})`}
              type="number"
              min={0}
              max={assignment?.maximumMarks}
              value={marks}
              onChange={(e) => setMarks(e.target.value)}
              error={fieldErrors.marks}
            />
            <Textarea
              label="Feedback"
              rows={3}
              value={feedback}
              onChange={(e) => setFeedback(e.target.value)}
            />
            <Select
              label="Status"
              value={status}
              onChange={(e) => setStatus(e.target.value as SubmissionStatus)}
              options={statusOptions}
            />
          </form>
        )}
      </Modal>
    </div>
  );
}
