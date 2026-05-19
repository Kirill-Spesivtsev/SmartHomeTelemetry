import { Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import ChartCard from "../../features/dashboard/chart-card";
import { Co2PmItem } from "../../types/models";

export default function CurrentAirPollutionChart({ data } : { data: Co2PmItem[] }) {
  return (
    <ChartCard title="Air CO₂ и PM2.5" subtitle="Air quality by room">
  <ResponsiveContainer width="100%" height="100%" >
    <BarChart data={data} >
      <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
      <XAxis dataKey="room" tick={{ fill: "#94a3b8", fontSize: 16 }} />
      <YAxis tick={{ fill: "#94a3b8", fontSize: 16 }} />
      <Tooltip contentStyle={{ background: "#0f172a", border: "1px solid #334155" }} />
      <Legend />
      <Bar dataKey="co2" name="CO₂" fill="#a78bfa" radius={[4, 4, 0, 0]} maxBarSize={28} />
      <Bar dataKey="pm25" name="PM2.5" fill="#34d399" radius={[4, 4, 0, 0]} maxBarSize={28} />
    </BarChart>
  </ResponsiveContainer>
</ChartCard>
  );
}

