import { formatAxisDayTime } from "../../features/dashboard/dashboard-chart-utils";
import { MotionRow } from "../../types/models";

export default function LastMotionTable({ rows } : { rows: MotionRow[] }) {
  return (
    <div className="overflow-hidden rounded-xl border border-slate-700/70 bg-slate-900/50">
      <div className="border-b border-slate-700/80 bg-slate-800/40 px-3 py-2 text-sm font-medium text-violet-300">
        Motion
      </div>
      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs text-slate-300">
          <thead className="bg-slate-800/30 text-slate-400">
            <tr>
              <th className="px-3 py-2">Room</th>
              <th className="px-3 py-2">Motion</th>
              <th className="px-3 py-2">Time</th>
            </tr>
          </thead>
          <tbody>
            {rows.map(
              (row: MotionRow) => (
                <tr key={`${row.locationName}-room`} className="border-t border-slate-800/80">
                  <td className="px-3 py-2 font-medium text-slate-200">{row.locationName}</td>
                  <td className="px-3 py-2">{row.motionDetected ? "yes" : "no"}</td>
                  <td className="px-3 py-2 text-slate-500">{formatAxisDayTime(row.createdAtUtc)}</td>
                </tr>
              ),
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}


