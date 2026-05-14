import { CartesianGrid, Legend, ResponsiveContainer, Scatter, ScatterChart, Tooltip, XAxis, YAxis, ZAxis } from "recharts";
import ChartCard from "../../features/dashboard/chart-card";
import { chartPalette } from "../../features/dashboard/dashboard-chart-utils";
import { GroupedAirScatterItem } from "../../types/models";

export default function AirPollutionHistoryChart({ data } : { data: GroupedAirScatterItem[] }) {
  return (
    <ChartCard title="Air pollution" subtitle="PM2.5 and CO₂ distribution">
      <ResponsiveContainer width="100%" height="100%">
        <ScatterChart>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
          <XAxis
            type="number"
            dataKey="co2"
            name="CO₂"
            tick={{ fill: "#94a3b8", fontSize: 16 }}
          />
          <YAxis
            type="number"
            dataKey="pm25"
            name="PM2.5"
            tick={{ fill: "#94a3b8", fontSize: 16 }}
          />
          <ZAxis range={[60, 60]} />
          <Tooltip
            cursor={{ strokeDasharray: "3 3" }}
            contentStyle={{ background: "#0f172a", border: "1px solid #334155" }}
          />
          {data.map((group: any, index: number) => (
            <Scatter
              key={group.name}
              name={group.name}
              data={group.data}
              fill={chartPalette[index % chartPalette.length]}
            />
          ))}
          <Legend />
        </ScatterChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

