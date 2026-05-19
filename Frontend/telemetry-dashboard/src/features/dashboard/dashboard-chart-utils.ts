import type { AirQualityMetric, EnergyMetric} from "../../types/data"
import { ChartMultipoint, RangeKey } from "../../types/models";

export const chartPalette = [
  "#38bdf8",
  "#a78bfa",
  "#34d399",
  "#fb923c",
  "#f472b6",
  "#facc15",
  "#2dd4bf",
  "#60a5fa",
];

export function rangeStartUtc(key: RangeKey): Date {
  const hours = key === "1h" ? 1 : key === "6h" ? 6 : 24;
  return new Date(Date.now() - hours * 3600 * 1000);
}


export function formatAxisTime(iso: string): string {
  const d = new Date(iso);
  return d.toLocaleTimeString(undefined, { hour: "2-digit", minute: "2-digit" });
}

export function formatAxisDayTime(iso: string): string {
  const d = new Date(iso);
  return d.toLocaleString(undefined, {
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

export function airSeriesByLocation(
  rows: AirQualityMetric[],
  key: "co2" | "pm25" | "humidity",
): ChartMultipoint[] {
  const times = Array.from(new Set(rows.map((r) => r.createdAt))).sort(
    (a, b) => new Date(a).getTime() - new Date(b).getTime(),
  );

  const rooms = Array.from(new Set(rows.map((r) => r.location.name))).sort();

  return times.map((time) => {
    const row: ChartMultipoint = { time };

    const atTime = rows.filter((r) => r.createdAt === time);

    for (const room of rooms) {
      const hit = atTime.find((r) => r.location.name === room);
      row[room] = hit ? hit[key] : NaN;
    }

    return row;
  });
}


export function energySeriesByLocation(rows: EnergyMetric[]): ChartMultipoint[] {
  const times = Array.from(new Set(rows.map((r) => r.createdAt))).sort(
    (a, b) => new Date(a).getTime() - new Date(b).getTime(),
  );

  const rooms = Array.from(new Set(rows.map((r) => r.location.name))).sort();

  return times.map((time) => {
    const row: ChartMultipoint = { time };

    const atTime = rows.filter((r) => r.createdAt === time);

    for (const room of rooms) {
      const hit = atTime.find((r) => r.location.name === room);
      row[room] = hit ? Number(hit.energy.toFixed(2)) : NaN;
    }

    return row;
  });
}

