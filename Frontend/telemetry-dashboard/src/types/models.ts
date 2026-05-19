export type RangeKey = "1h" | "6h" | "24h";

export interface AirRow {
  locationName: string;
  co2: number;
  pm25: number;
  humidity: number;
  createdAtUtc: string;
};

export interface EnergyRow {
  locationName: string;
  energy: number | null;
  createdAtUtc: string;
};

export interface MotionRow {
  locationName: string;
  motionDetected: boolean;
  createdAtUtc: string;
};

export interface  MotionBarItem {
  room: string;
  motion: number;
};

export interface HumidityRadialItem {
  name: string;
  humidity: number;
  fill: string;
};

export interface RadarEnergyItem {
  room: string;
  energy: number;
};

export interface Co2PmItem {
  room: string;
  co2: number;
  pm25: number;
};

export interface AirScatterItem {
  co2: number;
  pm25: number;
  name: string;
};

export interface GroupedAirScatterItem {
  name: string;
  data: AirScatterItem[];
};

export type ChartMultipoint = {
  time: string;
  [room: string]: number | string;
};