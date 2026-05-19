import { gql } from "@apollo/client";

export const GET_AIR_QUALITY_METRICS = gql`
  query GetAirQualityMetrics($fromUtc: DateTime){
    airQualityMetrics(
      where: { createdAt: { gte: $fromUtc } }
      order: { createdAt: DESC }
      first: 500
    ) {
      nodes {
        id
        createdAt
        co2
        pm25
        humidity
        location {
          name
        }
      }
    }
  }
`;

export const GET_ENERGY_METRICS = gql`
  query GetEnergyMetrics($fromUtc: DateTime){
    energyMetrics(
      where: { createdAt: { gte: $fromUtc } }
      order: { createdAt: DESC }
      first: 500
    ) {
      nodes {
        id
        createdAt
        energy
        location {
          name
        }
      }
    }
  }
`;

export const GET_MOTION_METRICS = gql`
  query GetMotionMetrics($fromUtc: DateTime){
    motionMetrics(
      where: { createdAt: { gte: $fromUtc } }
      order: { createdAt: DESC }
      first: 500
    ) {
      nodes {
        id
        createdAt
        motionDetected
        location {
          name
        }
      }
    }
  }
`;

export const GET_LATEST_AIR_QUALITY = gql`
  query LatestAirQuality($fromUtc: DateTime) {
    latestAirQualityMetrics(fromUtc: $fromUtc) {
      locationId
      locationName
      type
      createdAtUtc
      co2
      pm25
      humidity
    }
  }
`;

export const GET_LATEST_ENERGY = gql`
  query LatestEnergy($fromUtc: DateTime) {
    latestEnergyMetrics(fromUtc: $fromUtc) {
      locationId
      locationName
      type
      createdAtUtc
      energy
    }
  }
`;

export const GET_LATEST_MOTION = gql`
  query LatestMotion($fromUtc: DateTime) {
    latestMotionMetrics(fromUtc: $fromUtc) {
      locationId
      locationName
      type
      createdAtUtc
      motionDetected
    }
  }
`;

export const GET_ENERGY_AGGREGATES = gql`
  query EnergyAggregates($fromUtc: DateTime, $toUtc: DateTime) {
    energyAggregatesByLocation(fromUtc: $fromUtc, toUtc: $toUtc) {
      locationId
      locationName
      count
      avgEnergy
      minEnergy
      maxEnergy
    }
  }
`;

export const GET_AIR_QUALITY_AGGREGATES = gql`
  query AirQualityAggregates($fromUtc: DateTime, $toUtc: DateTime) {
    airQualityAggregatesByLocation(fromUtc: $fromUtc, toUtc: $toUtc) {
      locationId
      locationName
      count
      avgCo2
      minCo2
      maxCo2
      avgPm25
      minPm25
      maxPm25
      avgHumidity
      minHumidity
      maxHumidity
    }
  }
`;
