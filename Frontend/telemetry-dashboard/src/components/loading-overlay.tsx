export function LoadingOverlay({ visible, label } : { visible: boolean, label?: string }) {
  if (!visible) return null;
  return (
    <div
      className="fixed inset-0 z-50 flex flex-col items-center justify-center gap-3 bg-slate-950/75 backdrop-blur-sm"
      role="status"
      aria-live="polite"
      aria-busy="true"
    >
      <div className="h-12 w-12 animate-spin rounded-full border-4 border-sky-500/30 border-t-sky-400" />
      <p className="text-sm font-medium text-slate-300">{label ?? "Loading Data…"}</p>
    </div>
  );
}
