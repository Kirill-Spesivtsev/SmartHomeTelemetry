import { CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import ChartCard from "../../features/dashboard/chart-card";
import { chartPalette, formatAxisDayTime, formatAxisTime } from "../../features/dashboard/dashboard-chart-utils";

export default function EnergyHistoryChart({ data, legend } : { data: Record<string, unknown>[], legend: string[] }) {
  return (
    <ChartCard title="Energy by time" subtitle="Energy (kW·h)">
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
          <XAxis
            dataKey="time"
            tick={{ fill: "#94a3b8", fontSize: 16 }}
            tickFormatter={(v) => formatAxisTime(String(v))}
          />
          <YAxis tick={{ fill: "#94a3b8", fontSize: 16 }} />
          <Tooltip
            contentStyle={{ background: "#0f172a", border: "1px solid #334155" }}
            labelFormatter={(v) => formatAxisDayTime(String(v))}
            formatter={(value, name) => {
              if (value == null || isNaN(Number(value))) return null;
              return [value, name];
            }}
          />
          <Legend wrapperStyle={{ fontSize: 16 }} />
          {legend.map((room, i) => (
            <Line
              key={room}
              type="monotone"
              dataKey={room}
              stroke={chartPalette[i % chartPalette.length]}
              dot={false}
              strokeWidth={2}
              connectNulls
            />
          ))}
        </LineChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

