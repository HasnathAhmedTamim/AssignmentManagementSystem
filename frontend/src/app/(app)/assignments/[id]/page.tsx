"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { api, ApiError } from "@/lib/api";
import type { Assignment, Submission } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Badge, statusTone } from "@/components/ui/Badge";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";
import {
  formatDate,
  fromDateTimeLocalValue,
  toDateTimeLocalValue,
} from "@/lib/format";

export default function AssignmentDetailPage() {
  const params = useParams<{ id: string }>();
  const id = params.id;
  const { user } = useAuth();
  const router = useRouter();

  const [assignment, setAssignment] = useState<Assignment | null>(null);
  const [submission, setSubmission] = useState<Submission | null>(null);
  const [adminSubs, setAdminSubs] = useState<Submission[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({
    title: "",
    description: "",
    deadline: "",
    maximumMarks: "",
  });
  const [answer, setAnswer] = useState("");
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const data = await api.getAssignment(id);
      setAssignment(data);
      setForm({
        title: data.title,
        description: data.description,
        deadline: toDateTimeLocalValue(data.deadline),
        maximumMarks: String(data.maximumMarks),
      });

      if (user?.role === "Student" && data.mySubmission) {
        const sub = await api.getSubmission(data.mySubmission.id);
        setSubmission(sub);
        setAnswer(sub.answer);
      } else {
        setSubmission(null);
        setAnswer("");
      }

      if (user?.role === "Admin") {
        setAdminSubs(await api.getSubmissionsByAssignment(id));
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load assignment");
    } finally {
      setLoading(false);
    }
  }, [id, user?.role]);

  useEffect(() => {
    load();
  }, [load]);

  async function saveEdit(e: FormEvent) {
    e.preventDefault();
    if (!assignment) return;
    const next: Record<string, string> = {};
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
    setSuccess("");
    try {
      await api.updateAssignment(assignment.id, {
        title: form.title.trim(),
        description: form.description.trim(),
        deadline: fromDateTimeLocalValue(form.deadline),
        maximumMarks: marks,
      });
      setEditing(false);
      setSuccess("Assignment updated.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Update failed");
    } finally {
      setSaving(false);
    }
  }

  async function publish() {
    if (!assignment || !confirm("Publish this assignment to students?")) return;
    try {
      await api.publishAssignment(assignment.id);
      setSuccess("Assignment published.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Publish failed");
    }
  }

  async function remove() {
    if (!assignment || !confirm("Delete this assignment?")) return;
    try {
      await api.deleteAssignment(assignment.id);
      router.push("/assignments");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Delete failed");
    }
  }

  async function submitAnswer(e: FormEvent) {
    e.preventDefault();
    if (!assignment) return;
    if (!answer.trim()) {
      setFieldErrors({ answer: "Answer is required" });
      return;
    }
    setFieldErrors({});
    setSaving(true);
    setError("");
    setSuccess("");
    try {
      if (submission && submission.canUpdate) {
        await api.updateSubmission(submission.id, { answer: answer.trim() });
        setSuccess("Submission updated.");
      } else if (!submission) {
        await api.createSubmission({
          assignmentId: assignment.id,
          answer: answer.trim(),
        });
        setSuccess("Submission sent.");
      }
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Submit failed");
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <Spinner />;
  if (!assignment) {
    return (
      <div>
        <Alert>{error || "Assignment not found."}</Alert>
        <div className="mt-4">
          <Link href="/assignments">
            <Button variant="secondary">Back</Button>
          </Link>
        </div>
      </div>
    );
  }

  const teacherCanEdit = user?.role === "Teacher";
  const canSubmit =
    user?.role === "Student" &&
    assignment.status === "Published" &&
    (!submission || submission.canUpdate);

  return (
    <div>
      <PageHeader
        title={assignment.title}
        description={`${assignment.subjectName} · ${assignment.classRoomName} · ${assignment.teacherName}`}
        actions={
          <div className="flex flex-wrap gap-2">
            {teacherCanEdit && assignment.status === "Draft" && (
              <Button onClick={publish}>Publish</Button>
            )}
            {teacherCanEdit && (
              <Button
                variant="secondary"
                onClick={() => setEditing((v) => !v)}
              >
                {editing ? "Cancel edit" : "Edit"}
              </Button>
            )}
            {teacherCanEdit && (
              <Link href={`/assignments/${assignment.id}/submissions`}>
                <Button variant="secondary">Submissions</Button>
              </Link>
            )}
            {teacherCanEdit && (
              <Button variant="danger" onClick={remove}>
                Delete
              </Button>
            )}
            {user?.role === "Admin" && (
              <Link href={`/assignments/${assignment.id}/submissions`}>
                <Button variant="secondary">View submissions</Button>
              </Link>
            )}
          </div>
        }
      />

      {error && (
        <div className="mb-4">
          <Alert>{error}</Alert>
        </div>
      )}
      {success && (
        <div className="mb-4">
          <Alert variant="success">{success}</Alert>
        </div>
      )}

      <div className="mb-4 flex flex-wrap gap-2">
        <Badge tone={statusTone(assignment.status)}>{assignment.status}</Badge>
        <Badge tone="neutral">Deadline {formatDate(assignment.deadline)}</Badge>
        <Badge tone="neutral">Max {assignment.maximumMarks} marks</Badge>
      </div>

      {editing && teacherCanEdit ? (
        <form
          onSubmit={saveEdit}
          className="space-y-4 rounded-lg border border-slate-200 bg-white p-5 shadow-sm"
          noValidate
        >
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
          <Button type="submit" loading={saving}>
            Save changes
          </Button>
        </form>
      ) : (
        <div className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <h2 className="text-sm font-medium text-slate-500">Description</h2>
          <p className="mt-2 whitespace-pre-wrap text-sm leading-relaxed text-slate-800">
            {assignment.description}
          </p>
        </div>
      )}

      {user?.role === "Student" && (
        <div className="mt-6 rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <h2 className="font-display text-lg font-semibold text-slate-900">
            Your submission
          </h2>
          {submission && (
            <div className="mt-3 flex flex-wrap gap-2">
              <Badge tone={statusTone(submission.status)}>
                {submission.status}
              </Badge>
              <Badge tone="neutral">
                Submitted {formatDate(submission.submittedAt)}
              </Badge>
              {submission.marks != null && (
                <Badge tone="teal">
                  Marks: {submission.marks}/{assignment.maximumMarks}
                </Badge>
              )}
            </div>
          )}
          {submission?.feedback && (
            <div className="mt-3 rounded-md bg-slate-50 px-3 py-2 text-sm text-slate-700">
              <span className="font-medium">Feedback: </span>
              {submission.feedback}
            </div>
          )}

          {canSubmit ? (
            <form onSubmit={submitAnswer} className="mt-4 space-y-3" noValidate>
              <Textarea
                label={submission ? "Update answer" : "Your answer"}
                rows={6}
                value={answer}
                onChange={(e) => setAnswer(e.target.value)}
                error={fieldErrors.answer}
              />
              <Button type="submit" loading={saving}>
                {submission ? "Update submission" : "Submit"}
              </Button>
            </form>
          ) : submission ? (
            <div className="mt-4">
              <h3 className="text-sm font-medium text-slate-500">Answer</h3>
              <p className="mt-1 whitespace-pre-wrap text-sm text-slate-800">
                {submission.answer}
              </p>
              {!submission.canUpdate && (
                <p className="mt-2 text-xs text-slate-500">
                  This submission can no longer be updated.
                </p>
              )}
            </div>
          ) : assignment.status !== "Published" ? (
            <p className="mt-3 text-sm text-slate-500">
              This assignment is not published yet.
            </p>
          ) : (
            <p className="mt-3 text-sm text-slate-500">
              Deadline has passed or submissions are closed.
            </p>
          )}
        </div>
      )}

      {user?.role === "Admin" && (
        <div className="mt-6 rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <div className="mb-3 flex items-center justify-between gap-3">
            <h2 className="font-display text-lg font-semibold text-slate-900">
              Submissions
            </h2>
            <Link href={`/assignments/${assignment.id}/submissions`}>
              <Button variant="secondary">Open grading view</Button>
            </Link>
          </div>
          {adminSubs.length ? (
            <ul className="space-y-2 text-sm">
              {adminSubs.map((s) => (
                <li
                  key={s.id}
                  className="flex flex-wrap items-center justify-between gap-2 border-t border-slate-100 py-2 first:border-0"
                >
                  <span className="font-medium text-slate-800">
                    {s.studentName}
                  </span>
                  <span className="text-slate-500">
                    {s.status}
                    {s.marks != null ? ` · ${s.marks}` : ""}
                  </span>
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-sm text-slate-500">No submissions yet.</p>
          )}
        </div>
      )}

    </div>
  );
}
