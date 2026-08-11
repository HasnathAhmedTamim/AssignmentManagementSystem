import { ReactNode } from "react";
import { PageHeader } from "@/components/ui/PageHeader";
import { Alert } from "@/components/ui/Alert";
import { Spinner } from "@/components/ui/Spinner";

interface AdminPageShellProps {
  title: string;
  description: string;
  actions?: ReactNode;
  loading?: boolean;
  error?: string;
  children: ReactNode;
}

export function AdminPageShell({
  title,
  description,
  actions,
  loading = false,
  error,
  children,
}: AdminPageShellProps) {
  if (loading) return <Spinner />;

  return (
    <div>
      <PageHeader title={title} description={description} actions={actions} />
      {error ? (
        <div className="mb-4">
          <Alert>{error}</Alert>
        </div>
      ) : null}
      {children}
    </div>
  );
}
