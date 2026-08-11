"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { api } from "@/lib/api";
import type { Assignment, Submission } from "@/lib/types";
import { PageHeader } from "@/components/ui/PageHeader";
import { Spinner } from "@/components/ui/Spinner";
import { Alert } from "@/components/ui/Alert";

function StatCard({
  label,
  value,
  hint,
}: {
  label: string;
  value: string | number;
  hint?: string;
}) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
      <p className="text-sm text-slate-500">{label}</p>
      <p className="mt-2 font-display text-3xl font-semibold text-slate-900">
        {value}
      </p>
      {hint && <p className="mt-1 text-xs text-slate-500">{hint}</p>}
    </div>
  );
}

function QuickLink({ href, label }: { href: string; label: string }) {
  return (
    <Link
      href={href}
      className="rounded-md border border-slate-200 bg-white px-4 py-3 text-sm font-medium text-slate-700 shadow-sm transition hover:border-teal-700/40 hover:text-teal-800"
    >
      {label}
    </Link>
  );
}

export default function DashboardPage() {
  const { user } = useAuth();
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [counts, setCounts] = useState({
    users: 0,
    classes: 0,
    subjects: 0,
    enrollments: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let cancelled = false;
    async function load() {
      if (!user) return;
      setLoading(true);
      setError("");
      try {
        const asg = await api.getAssignments();
        if (cancelled) return;
        setAssignments(asg);

        if (user.role === "Admin") {
          const [users, classes, subjects, enrollments] = await Promise.all([
            api.getUsers(),
            api.getClassRooms(),
            api.getSubjects(),
            api.getEnrollments(),
          ]);
          if (cancelled) return;
          setCounts({
            users: users.length,
            classes: classes.length,
            subjects: subjects.length,
            enrollments: enrollments.length,
          });
        }

        if (user.role === "Student") {
          const mine = await api.getMySubmissions();
          if (cancelled) return;
          setSubmissions(mine);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err instanceof Error ? err.message : "Failed to load dashboard");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }
    load();
    return () => {
      cancelled = true;
    };
  }, [user]);

  if (loading) return <Spinner />;

  const published = assignments.filter((a) => a.status === "Published").length;
  const drafts = assignments.filter((a) => a.status === "Draft").length;

  return (
    <div>
      <PageHeader
        title={`Welcome, ${user?.fullName?.split(" ")[0] || "there"}`}
        description={`Signed in as ${user?.role}. Here's a quick overview.`}
      />

      {error && (
        <div className="mb-4">
          <Alert>{error}</Alert>
        </div>
      )}

      {user?.role === "Admin" && (
        <>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <StatCard label="Users" value={counts.users} />
            <StatCard label="Classes" value={counts.classes} />
            <StatCard label="Subjects" value={counts.subjects} />
            <StatCard label="Enrollments" value={counts.enrollments} />
          </div>
          <div className="mt-4 grid gap-4 sm:grid-cols-2">
            <StatCard label="Assignments" value={assignments.length} hint={`${published} published`} />
            <StatCard label="Drafts in system" value={drafts} />
          </div>
          <div className="mt-6 grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            <QuickLink href="/admin/users" label="Manage users" />
            <QuickLink href="/admin/classes" label="Manage classes" />
            <QuickLink href="/admin/subjects" label="Manage subjects" />
            <QuickLink href="/admin/teacher-assignments" label="Teacher assignments" />
            <QuickLink href="/admin/enrollments" label="Enrollments" />
            <QuickLink href="/assignments" label="View assignments" />
          </div>
        </>
      )}

      {user?.role === "Teacher" && (
        <>
          <div className="grid gap-4 sm:grid-cols-3">
            <StatCard label="My assignments" value={assignments.length} />
            <StatCard label="Published" value={published} />
            <StatCard label="Drafts" value={drafts} />
          </div>
          <div className="mt-6 grid gap-3 sm:grid-cols-2">
            <QuickLink href="/assignments/new" label="Create assignment" />
            <QuickLink href="/assignments" label="Browse assignments" />
          </div>
        </>
      )}

      {user?.role === "Student" && (
        <>
          <div className="grid gap-4 sm:grid-cols-3">
            <StatCard label="Published assignments" value={published} />
            <StatCard label="My submissions" value={submissions.length} />
            <StatCard
              label="Graded"
              value={submissions.filter((s) => s.status === "Reviewed").length}
            />
          </div>
          <div className="mt-6">
            <QuickLink href="/assignments" label="View assignments" />
          </div>
        </>
      )}
    </div>
  );
}
