import RangeToggle from "../../components/range-toggle";
import { RangeKey } from "../../types/models";
import { Dispatch, SetStateAction } from "react";

export default function DashboardHeader({ rangeKey, setRangeKey } : { rangeKey: RangeKey, setRangeKey: Dispatch<SetStateAction<RangeKey>> }) {
  return (
    <header className="border-b border-slate-800/80 bg-slate-900/40 backdrop-blur-md">
        <div className="mx-auto flex max-w-[1600px] flex-col gap-4 px-4 py-6 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-white sm:text-3xl">
              Smart Home Telemetry Dashboard
            </h1>
            <p className="mt-1 max-w-2xl text-sm text-slate-400">
              Latest values, time windows, and room aggregates
            </p>
          </div>
          <div className="flex flex-col items-start gap-2 sm:items-end">
            <span className="text-xs uppercase tracking-wider text-slate-500">Time period</span>
            <RangeToggle value={rangeKey} onChange={setRangeKey} />
          </div>
        </div>
      </header>
  );
}


