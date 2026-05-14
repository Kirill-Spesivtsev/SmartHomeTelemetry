import React, { useEffect, useMemo, useState } from "react";
import { useQuery } from "@apollo/client";
import {
  GET_AIR_QUALITY_AGGREGATES,
  GET_AIR_QUALITY_METRICS,
  GET_ENERGY_AGGREGATES,
  GET_ENERGY_METRICS,
  GET_LATEST_AIR_QUALITY,
  GET_LATEST_ENERGY,
  GET_LATEST_MOTION,
  GET_MOTION_METRICS,
} from "../../graphql/queries";
import { AirRow, Co2PmItem, EnergyRow, HumidityRadialItem, MotionBarItem, MotionRow, RadarEnergyItem, RangeKey } from "../../types/models";
import { chartPalette, rangeStartUtc } from "./dashboard-chart-utils";
import { GetAirAggregatesData, GetAirQualityMetricsData, GetEnergyAggregatesData, GetEnergyMetricsData, GetLatestAirData, GetLatestEnergyData, GetLatestMotionData, GetMotionMetricsData, QueryFromUtc, QueryWithRange } from "../../types/queries";
import CurrentMotionChart from "../../components/charts/current-motion-chart";
import CurrentEnergyChart from "../../components/charts/current-energy-chart";
import CurrentAirPollutionChart from "../../components/charts/current-air-pollution-chart";
import CurrentAirHumidityChart from "../../components/charts/current-air-humidity-chart";


export default function Dashboard() {
  const [rangeKey, setRangeKey] = useState<RangeKey>("1h");

  const fromUtc = useMemo<string>(
    () => rangeStartUtc(rangeKey).toISOString(),
    [rangeKey]
  );

  const airQualityMetrics = useQuery<GetAirQualityMetricsData, QueryFromUtc>(
    GET_AIR_QUALITY_METRICS,
    { variables: { fromUtc } }
  );

  const energyMetrics = useQuery<GetEnergyMetricsData, QueryFromUtc>(
    GET_ENERGY_METRICS,
    { variables: { fromUtc } }
  );

  const motionMetrics = useQuery<GetMotionMetricsData, QueryFromUtc>(
    GET_MOTION_METRICS,
    { variables: { fromUtc } }
  );

  const latestAir = useQuery<GetLatestAirData, QueryFromUtc>(
    GET_LATEST_AIR_QUALITY,
    { variables: { fromUtc } }
  );

  const latestEnergy = useQuery<GetLatestEnergyData, QueryFromUtc>(
    GET_LATEST_ENERGY,
    { variables: { fromUtc } }
  );

  const latestMotion = useQuery<GetLatestMotionData, QueryFromUtc>(
    GET_LATEST_MOTION,
    { variables: { fromUtc } }
  );

  const aggEnergy = useQuery<GetEnergyAggregatesData, QueryWithRange>(
    GET_ENERGY_AGGREGATES,
    { variables: { fromUtc } 
    }
  );

  const aggAir = useQuery<GetAirAggregatesData, QueryWithRange>(
    GET_AIR_QUALITY_AGGREGATES,
    { variables: { fromUtc } }
  );

  const airWindow = useQuery<GetAirQualityMetricsData, QueryFromUtc & { first?: number }>(
    GET_AIR_QUALITY_METRICS,
    { variables: { fromUtc, first: 500 } }
  );

  const energyWindow = useQuery<GetEnergyMetricsData, QueryFromUtc & { first?: number }>(
    GET_ENERGY_METRICS,
    { variables: { fromUtc, first: 500 } }
  );

  const airRows = useMemo<AirRow[]>(
    () => latestAir.data?.latestAirQualityMetrics ?? [],
    [latestAir.data],
  );

  const energyRows = useMemo<EnergyRow[]>(
    () => latestEnergy.data?.latestEnergyMetrics ?? [],
    [latestEnergy.data],
  );

  const motionRows = useMemo<MotionRow[]>(
    () => latestMotion.data?.latestMotionMetrics ?? [],
    [latestMotion.data],
  );

  const latestMotionData = useMemo<MotionBarItem[]>(
  () =>
    motionRows.map((m) => ({
      room: m.locationName,
      motion: m.motionDetected ? 1 : 0,
    })),
  [motionRows],
);

  const humidityRadial = useMemo<HumidityRadialItem[]>(
    () =>
      airRows.map((a, i) => ({
        name: a.locationName,
        humidity: a.humidity,
        fill: chartPalette[i % chartPalette.length],
      })),
    [airRows],
  );

  const radarChartEnergyData = useMemo<RadarEnergyItem[]>(
    () =>
      energyRows.map((e) => ({
        room: e.locationName,
        energy: e.energy ?? 0,
      })),
    [energyRows],
  );

  const co2PmGrouped = useMemo<Co2PmItem[]>(
    () =>
      airRows.map((a) => ({
        room: a.locationName,
        co2: a.co2,
        pm25: a.pm25,
      })),
    [airRows],
  );

  return (
    <div className="min-h-screen">

      <main className="mx-auto space-y-10 px-4 py-8">
        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white">Current metrics</h2>
          
          <div className="grid gap-4 lg:grid-cols-2">
            <CurrentMotionChart data={latestMotionData}/>

            <CurrentEnergyChart data={radarChartEnergyData}/>

            <CurrentAirPollutionChart data={co2PmGrouped}/>

            <CurrentAirHumidityChart data={humidityRadial}/>
          </div>
        </section>

        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white"> Telemetry history</h2>
        </section>

        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white">Last values</h2>
        </section>

        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white">Aggregates</h2>
        </section>
      </main>
    </div>
  );
}


