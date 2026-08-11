"use client";

import { FormEvent, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { ApiError } from "@/lib/api";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";

const demos = [
  { role: "Admin", email: "admin@school.com", password: "Admin@123" },
  { role: "Teacher", email: "teacher@school.com", password: "Teacher@123" },
  { role: "Student", email: "student@school.com", password: "Student@123" },
];

export default function LoginPage() {
  const { login, user, loading } = useAuth();
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<{ email?: string; password?: string }>(
    {}
  );
  const [formError, setFormError] = useState("");
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!loading && user) router.replace("/dashboard");
  }, [user, loading, router]);

  if (loading) return <Spinner />;
  if (user) return <Spinner label="Redirecting..." />;

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    const next: typeof errors = {};
    if (!email.trim()) next.email = "Email is required";
    else if (!/^\S+@\S+\.\S+$/.test(email)) next.email = "Enter a valid email";
    if (!password) next.password = "Password is required";
    setErrors(next);
    if (Object.keys(next).length) return;

    setSubmitting(true);
    setFormError("");
    try {
      await login(email.trim(), password);
    } catch (err) {
      setFormError(
        err instanceof ApiError ? err.message : "Unable to sign in. Try again."
      );
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="relative flex min-h-screen items-center justify-center px-4 py-10">
      <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,#ccfbf1_0%,transparent_40%),radial-gradient(circle_at_80%_0%,#e2e8f0_0%,transparent_35%),linear-gradient(180deg,#f8fafc_0%,#eef2f7_100%)]" />
      <div className="relative grid w-full max-w-4xl gap-6 lg:grid-cols-[1.1fr_0.9fr]">
        <section className="rounded-xl border border-slate-200/80 bg-white/80 p-8 shadow-sm backdrop-blur">
          <p className="font-display text-3xl font-semibold text-teal-800">
            CampusDesk
          </p>
          <h1 className="mt-3 font-display text-2xl font-semibold text-slate-900">
            Sign in to manage assignments
          </h1>
          <p className="mt-2 text-sm text-slate-600">
            A practical workspace for admins, teachers, and students.
          </p>

          <form onSubmit={onSubmit} className="mt-8 space-y-4" noValidate>
            {formError && <Alert>{formError}</Alert>}
            <Input
              label="Email"
              type="email"
              name="email"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              error={errors.email}
              placeholder="you@school.com"
            />
            <Input
              label="Password"
              type="password"
              name="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              error={errors.password}
              placeholder="••••••••"
            />
            <Button type="submit" className="w-full" loading={submitting}>
              Sign in
            </Button>
          </form>
        </section>

        <aside className="rounded-xl border border-slate-200/80 bg-white/70 p-6 shadow-sm backdrop-blur">
          <h2 className="font-display text-lg font-semibold text-slate-900">
            Demo credentials
          </h2>
          <p className="mt-1 text-sm text-slate-600">
            Click a role to fill the form.
          </p>
          <ul className="mt-4 space-y-3">
            {demos.map((d) => (
              <li key={d.role}>
                <button
                  type="button"
                  onClick={() => {
                    setEmail(d.email);
                    setPassword(d.password);
                    setErrors({});
                    setFormError("");
                  }}
                  className="w-full rounded-lg border border-slate-200 bg-slate-50 px-4 py-3 text-left transition hover:border-teal-700/40 hover:bg-teal-50/50"
                >
                  <p className="text-sm font-semibold text-teal-800">{d.role}</p>
                  <p className="mt-1 text-xs text-slate-600">{d.email}</p>
                  <p className="text-xs text-slate-500">{d.password}</p>
                </button>
              </li>
            ))}
          </ul>
        </aside>
      </div>
    </div>
  );
}
