export interface Location {
  name: string;
}

export interface EnergyMetric {
    id: string
    createdAt: string;
    location: Location
    energy: number;
}

export interface AirQualityMetric {
    id: string
    createdAt: string;
    location: Location
    co2: number;
    pm25: number;
    humidity: number;
}

export interface MotionMetric {
    id: string
    createdAt: string;
    location: Location
    motionDetected: boolean;
}

export interface LatestAirQualityDto {
  locationId: string;
  locationName: string;
  type: string;
  createdAtUtc: string;
  co2: number;
  pm25: number;
  humidity: number;
}

export interface LatestEnergyDto {
  locationId: string;
  locationName: string;
  type: string;
  createdAtUtc: string;
  energy: number | null;
}

export interface LatestMotionDto {
  locationId: string;
  locationName: string;
  type: string;
  createdAtUtc: string;
  motionDetected: boolean;
}

export interface EnergyAggregate {
  locationId: string;
  locationName: string;
  count: number;
  avgEnergy: number;
  minEnergy: number;
  maxEnergy: number;
}

export interface AirQualityAggregate {
  locationId: string;
  locationName: string;
  count: number;
  avgCo2: number;
  minCo2: number;
  maxCo2: number;
  avgPm25: number;
  minPm25: number;
  maxPm25: number;
  avgHumidity: number;
  minHumidity: number;
  maxHumidity: number;
}

export interface TelemetryEventItem {
  eventId: string;
  timestampUtc: string;
  metricType: number;
  locationName: string;
  payload: Record<string, unknown>;
}

export interface TelemetryBatchEvent {
  batchId: string;
  timestampUtc: string;
  items: TelemetryEventItem[];
}
