import { AirQualityAggregate, AirQualityMetric, EnergyAggregate, EnergyMetric, LatestAirQualityDto, LatestEnergyDto, LatestMotionDto, MotionMetric } from "./data";

export interface GetAirQualityMetricsData {
  airQualityMetrics: {
    nodes: AirQualityMetric[];
  };
}

export interface GetEnergyMetricsData {
  energyMetrics: {
    nodes: EnergyMetric[];
  };
}

export interface GetMotionMetricsData {
  motionMetrics: {
    nodes: MotionMetric[];
  };
}

export interface GetLatestAirData {
  latestAirQualityMetrics: LatestAirQualityDto[];
}

export interface GetLatestEnergyData {
  latestEnergyMetrics: LatestEnergyDto[];
}

export interface GetLatestMotionData {
  latestMotionMetrics: LatestMotionDto[];
}

export interface GetEnergyAggregatesData {
  energyAggregatesByLocation: EnergyAggregate[];
}

export interface GetAirAggregatesData {
  airQualityAggregatesByLocation: AirQualityAggregate[];
}

export interface QueryFromUtc {
  fromUtc?: string;
}

export interface QueryWithRange {
  fromUtc?: string;
  toUtc?: string;
}