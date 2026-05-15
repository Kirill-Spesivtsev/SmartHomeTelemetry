import { AirQualityAggregate } from "../../types/data";

export default function Pm25AggregateTable({ rows } : { rows: AirQualityAggregate[] }) {
  return (
    <div className="overflow-hidden rounded-xl border border-slate-700/70 bg-slate-900/50">
      <div className="border-b border-slate-700/80 bg-slate-800/40 px-3 py-2 text-sm font-medium text-sky-300">
        Air – PM2.5
      </div>
      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs text-slate-300">
          <thead className="bg-slate-800/30 text-slate-400">
            <tr>
              <th className="px-3 py-2">Room</th>
              <th className="px-3 py-2">Amount</th>
              <th className="px-3 py-2">Average</th>
              <th className="px-3 py-2">Min</th>
              <th className="px-3 py-2">Max</th>
            </tr>
          </thead>
          <tbody>
            {rows.map(
              (row: AirQualityAggregate) => (
                <tr key={`${row.locationName}-pm`} className="border-t border-slate-800/80">
                  <td className="px-3 py-2 font-medium text-slate-200">{row.locationName}</td>
                  <td className="px-3 py-2">{row.count}</td>
                  <td className="px-3 py-2">{row.avgPm25.toFixed(2)}</td>
                  <td className="px-3 py-2">{row.minPm25}</td>
                  <td className="px-3 py-2">{row.maxPm25}</td>
                </tr>
              ),
            )}
          </tbody>
        </table>
      </div>
    </div>

  );
}


