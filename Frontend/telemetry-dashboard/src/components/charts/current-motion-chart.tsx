import { Bar, BarChart, CartesianGrid, Cell, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import ChartCard from "../../features/dashboard/chart-card";
import { MotionBarItem } from "../../types/models";
import { chartPalette } from "../../features/dashboard/dashboard-chart-utils";

export default function CurrentMotionChart({ data } : { data: MotionBarItem[] }) {
  return (
    <ChartCard title="Motion" subtitle="Motion detection by room (0 / 1)">
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
          <XAxis dataKey="room" tick={{ fill: "#94a3b8", fontSize: 16 }} />
          <YAxis domain={[0, 1]} ticks={[0, 1]} tick={{ fill: "#94a3b8", fontSize: 16 }} />
          <Tooltip
            contentStyle={{ background: "#0f172a", border: "1px solid #334155" }}
            formatter={(v) => [Number(v) === 1 ? "yes" : "no", "motion"]}
            labelStyle={{ color: "#ffffff" }}
            itemStyle={{ color: "#aaaaaa" }}
          />
          <Bar dataKey="motion" radius={[4, 4, 0, 0]} maxBarSize={48}>
            {data.map((item, index) => (
              <Cell
                key={item.room}
                fill={chartPalette[index % chartPalette.length]}
              />
            ))}
          </Bar>
        </BarChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

