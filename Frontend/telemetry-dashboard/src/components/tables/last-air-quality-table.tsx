import { formatAxisDayTime } from "../../features/dashboard/dashboard-chart-utils";
import { AirRow } from "../../types/models";

export default function LastAirQualityTable({ rows } : { rows: AirRow[] }) {
  return (
    <div className="overflow-hidden rounded-xl border border-slate-700/70 bg-slate-900/50">
      <div className="border-b border-slate-700/80 bg-slate-800/40 px-3 py-2 text-sm font-medium text-sky-300">
        Air quality
      </div>
      <div className="overflow-x-auto">
        <table className="w-full text-left text-xs text-slate-300">
          <thead className="bg-slate-800/30 text-slate-400">
            <tr>
              <th className="px-3 py-2">Room</th>
              <th className="px-3 py-2">CO₂</th>
              <th className="px-3 py-2">PM2.5</th>
              <th className="px-3 py-2">Humidity</th>
              <th className="px-3 py-2">Time</th>
            </tr>
          </thead>
          <tbody>
            {rows.map(
              (row: AirRow) => (
                <tr key={`${row.locationName}-room`} className="border-t border-slate-800/80">
                  <td className="px-3 py-2 font-medium text-slate-200">{row.locationName}</td>
                  <td className="px-3 py-2">{row.co2}</td>
                  <td className="px-3 py-2">{row.pm25}</td>
                  <td className="px-3 py-2">{row.humidity}%</td>
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


