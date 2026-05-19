import { Legend, PolarAngleAxis, PolarGrid,  RadialBar, RadialBarChart, ResponsiveContainer, Tooltip } from "recharts";
import ChartCard from "../../features/dashboard/chart-card";
import { HumidityRadialItem } from "../../types/models";

const PolarAngleAxisFix = PolarAngleAxis as React.ElementType;

export default function CurrentAirHumidityChart({ data } : { data: HumidityRadialItem[] }) {
  return (
    <ChartCard title="Air humidity" subtitle="Humidity %">
      <ResponsiveContainer width="100%" height="100%">
        <RadialBarChart
          cx="50%"
          cy="50%"
          innerRadius="18%"
          outerRadius="90%"
          data={data}
          startAngle={90}
          endAngle={-270}
        >
          <PolarAngleAxisFix
            type="number"
            domain={[0, 100]}
            tick={false}
            axisLine={false}
          />
          <PolarGrid gridType="circle" stroke="#334155" />
          <RadialBar background dataKey="humidity" cornerRadius={6} />
          <Legend iconSize={10} wrapperStyle={{ fontSize: 16 }} />
          <Tooltip 
            contentStyle={{ background: "#0f172a", border: "1px solid #334155" }} 
            formatter={(value, name) => [
              <span style={{ color: "#94a3b8" }}>humidity: {value}%</span>,
            ]}
            labelFormatter={(_, payload) => (
              <span style={{ color: "#ffffff" }}>{payload?.[0]?.payload?.name}</span>
            )}
          />
        </RadialBarChart>
      </ResponsiveContainer>
    </ChartCard>
  );
}

