import { PolarAngleAxis, PolarGrid, PolarRadiusAxis, Radar, RadarChart, ResponsiveContainer, Tooltip } from "recharts";
import ChartCard from "../../features/dashboard/chart-card";
import { RadarEnergyItem } from "../../types/models";

const PolarAngleAxisFix = PolarAngleAxis as React.ElementType;
const PolarRadiusAxisFix = PolarRadiusAxis as React.ElementType;

export default function CurrentEnergyChart({ data } : { data: RadarEnergyItem[] }) {
  return (
    <ChartCard title="Energy" subtitle="Evergy values by room (kW·h)">
      <ResponsiveContainer width="100%" height="100%">
        <RadarChart cx="50%" cy="50%" outerRadius={110} data={data}>
          <PolarGrid stroke="#334155" />
          <PolarAngleAxisFix dataKey="room" tick={{ fill: "#94a3b8", fontSize: 16 }}/>
          <PolarRadiusAxisFix tick={{ fill: "#64748b", fontSize: 14 }} axisLine={false}/>
          <Radar
            name="energy"
            dataKey="energy"
            stroke="#38bdf8"
            fill="#38bdf8"
            fillOpacity={0.35}
          />
          <Tooltip contentStyle={{ background: "#0f172a", border: "1px solid #334155" }} />
        </RadarChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

