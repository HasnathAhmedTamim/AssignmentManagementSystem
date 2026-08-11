type Tone = "neutral" | "teal" | "amber" | "rose" | "slate";

const tones: Record<Tone, string> = {
  neutral: "bg-slate-100 text-slate-700",
  teal: "bg-teal-50 text-teal-800 ring-1 ring-teal-100",
  amber: "bg-amber-50 text-amber-800 ring-1 ring-amber-100",
  rose: "bg-rose-50 text-rose-700 ring-1 ring-rose-100",
  slate: "bg-slate-200/70 text-slate-700",
};

export function Badge({
  children,
  tone = "neutral",
}: {
  children: React.ReactNode;
  tone?: Tone;
}) {
  return (
    <span
      className={`inline-flex items-center rounded px-2 py-0.5 text-xs font-medium ${tones[tone]}`}
    >
      {children}
    </span>
  );
}

export function statusTone(status: string): Tone {
  switch (status) {
    case "Published":
    case "Reviewed":
    case "Admin":
      return "teal";
    case "Draft":
    case "Pending":
    case "Teacher":
      return "amber";
    case "Late":
    case "Student":
      return "rose";
    default:
      return "neutral";
  }
}
