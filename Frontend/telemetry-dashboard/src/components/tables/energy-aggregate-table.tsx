import { EnergyAggregate } from "../../types/data";

export default function EnergyAggregateTable({ rows } : { rows: EnergyAggregate[] }) {
  return (
    <div className="mb-6 overflow-hidden rounded-xl border border-slate-700/70 bg-slate-900/50">
      <div className="border-b border-slate-700/80 bg-slate-800/40 px-3 py-2 text-sm font-medium text-emerald-300">
        Energy
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
              (row: EnergyAggregate) => (
                <tr key={`${row.locationName}-room`} className="border-t border-slate-800/80">
                  <td className="px-3 py-2 font-medium text-slate-200">{row.locationName}</td>
                  <td className="px-3 py-2">{row.count}</td>
                  <td className="px-3 py-2">{row.avgEnergy.toFixed(2)}</td>
                  <td className="px-3 py-2">{row.minEnergy.toFixed(2)}</td>
                  <td className="px-3 py-2">{row.maxEnergy.toFixed(2)}</td>
                </tr>
              ),
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}


