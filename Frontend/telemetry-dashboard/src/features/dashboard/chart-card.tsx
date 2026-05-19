export default function ChartCard({
  title,
  subtitle,
  children,
  className,
}: {
  title: string;
  subtitle?: string;
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <section
      className={`rounded-xl border border-slate-700/70 bg-slate-900/50 p-4 shadow-lg shadow-slate-950/40 backdrop-blur-sm ${className ?? ""}`}
    >
      <div className="mb-2">
        <h3 className="text-lg font-semibold tracking-wide text-slate-200">{title}</h3>
        {subtitle ? <p className="mt-0.5 text-base text-slate-500">{subtitle}</p> : null}
      </div>
      <div className="h-72 w-full min-h-[18rem]">{children}</div>
    </section>
  );
}