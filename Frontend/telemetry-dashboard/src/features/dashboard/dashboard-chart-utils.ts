import { RangeKey } from "../../types/models";

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
