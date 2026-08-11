export function Alert({
  children,
  variant = "error",
}: {
  children: React.ReactNode;
  variant?: "error" | "success" | "info";
}) {
  const styles =
    variant === "error"
      ? "border-rose-200 bg-rose-50 text-rose-800"
      : variant === "success"
        ? "border-teal-200 bg-teal-50 text-teal-800"
        : "border-slate-200 bg-slate-50 text-slate-700";

  return (
    <div className={`rounded-md border px-3 py-2 text-sm ${styles}`}>
      {children}
    </div>
  );
}
