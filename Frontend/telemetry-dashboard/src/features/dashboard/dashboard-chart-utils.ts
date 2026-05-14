
import { RangeKey } from "../../types/models";

export function rangeStartUtc(key: RangeKey): Date {
  const hours = key === "1h" ? 1 : key === "6h" ? 6 : 24;
  return new Date(Date.now() - hours * 3600 * 1000);
}
