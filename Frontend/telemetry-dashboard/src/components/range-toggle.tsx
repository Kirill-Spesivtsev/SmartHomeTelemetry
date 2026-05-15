import { RangeKey } from "../types/models";

export default function RangeToggle({
  value,
  onChange,
}: {
  value: RangeKey;
  onChange: (v: RangeKey) => void;
}) {
  const keys: RangeKey[] = ["1h", "6h", "24h"];
  return (
    <div className="inline-flex rounded-lg border border-slate-600 bg-slate-900/80 p-0.5">
      {keys.map((k) => (
        <button
          key={k}
          type="button"
          onClick={() => onChange(k)}
          className={`rounded-md px-3 py-1.5 text-xs font-medium transition ${
            value === k
              ? "bg-sky-600 text-white shadow"
              : "text-slate-400 hover:bg-slate-800 hover:text-slate-200"
          }`}
        >
          {k}
        </button>
      ))}
    </div>
  );
}