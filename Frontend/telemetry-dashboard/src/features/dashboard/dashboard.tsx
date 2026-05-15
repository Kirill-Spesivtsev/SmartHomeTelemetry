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
import { AirRow, AirScatterItem, Co2PmItem, EnergyRow, GroupedAirScatterItem, HumidityRadialItem, MotionBarItem, MotionRow, RadarEnergyItem, RangeKey } from "../../types/models";
import { airSeriesByLocation, chartPalette, energySeriesByLocation, rangeStartUtc } from "./dashboard-chart-utils";
import { GetAirAggregatesData, GetAirQualityMetricsData, GetEnergyAggregatesData, GetEnergyMetricsData, GetLatestAirData, GetLatestEnergyData, GetLatestMotionData, GetMotionMetricsData, QueryFromUtc, QueryWithRange } from "../../types/queries";
import CurrentMotionChart from "../../components/charts/current-motion-chart";
import CurrentEnergyChart from "../../components/charts/current-energy-chart";
import CurrentAirPollutionChart from "../../components/charts/current-air-pollution-chart";
import CurrentAirHumidityChart from "../../components/charts/current-air-humidity-chart";
import AirCo2HistoryChart from "../../components/charts/air-co2-history-chart";
import AirPm25HistoryChart from "../../components/charts/air-pm25-history-chart";
import AirHumidityHistoryChart from "../../components/charts/air-humidity-history-chart";
import EnergyHistoryChart from "../../components/charts/energy-history-chart";
import AirPollutionHistoryChart from "../../components/charts/air-pollution-history-chart";
import { AirQualityAggregate, AirQualityMetric, EnergyAggregate, EnergyMetric } from "../../types/data";
import LastAirQualityTable from "../../components/tables/last-air-quality-table";
import LastEnergyTable from "../../components/tables/last-energy-table";
import LastMotionTable from "../../components/tables/last-motion-table";
import EnergyAggregateTable from "../../components/tables/energy-aggregate-table";
import Co2AggregateTable from "../../components/tables/co2-aggregate-table";
import Pm25AggregateTable from "../../components/tables/pm25-aggregate-table";
import HumidityAggregateTable from "../../components/tables/humidity-aggregate-table";


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

  const airHistory = useMemo<AirQualityMetric[]>(
    () => airWindow.data?.airQualityMetrics?.nodes ?? [],
    [airWindow.data],
  );

  const energyHistory = useMemo<EnergyMetric[]>(
    () => energyWindow.data?.energyMetrics?.nodes ?? [],
    [energyWindow.data],
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

  const roomsFromAirHistory = useMemo<string[]>(
    () => {
      const names = airHistory.map((r) => r.location.name);
      return Array.from(new Set(names)).sort();
    },
    [airHistory],
  );

  const co2Series = useMemo(
    () => airSeriesByLocation(airHistory, "co2"),
    [airHistory],
  );

  const pm25Series = useMemo(
    () => airSeriesByLocation(airHistory, "pm25"),
    [airHistory],
  );

  const humiditySeries = useMemo(
    () => airSeriesByLocation(airHistory, "humidity"),
    [airHistory],
  );

  const energySeries = useMemo(
    () => energySeriesByLocation(energyHistory),
    [energyHistory],
  );

  const energyRooms = useMemo<string[]>(
    () => {
      const names = energyHistory.map((r) => r.location.name);
      return Array.from(new Set(names)).sort();
    },
    [energyHistory],
  );

  const scatterPmCo2 = useMemo<AirScatterItem[]>(
    () =>
      airHistory.map((r) => ({
        co2: r.co2,
        pm25: r.pm25,
        name: r.location.name,
      })),
    [airHistory],
  );

  const groupedAirScatter = useMemo<GroupedAirScatterItem[]>(
    () =>
      Object.values(
        scatterPmCo2.reduce<Record<string, GroupedAirScatterItem>>((acc, item) => {
          if (!acc[item.name]) {
            acc[item.name] = { name: item.name, data: [] };
          }
          acc[item.name].data.push(item);
          return acc;
        }, {}),
      ).sort((a, b) => a.name.localeCompare(b.name)),
    [scatterPmCo2],
  );

    const airAggregatedRows = useMemo<AirQualityAggregate[]>(
    () => aggAir.data?.airQualityAggregatesByLocation ?? [],
    [aggAir.data],
  );

  const energyAggregatedRows = useMemo<EnergyAggregate[]>(
    () => aggEnergy.data?.energyAggregatesByLocation ?? [],
    [aggEnergy.data],
  );

  const anyError =
    latestAir.error ||
    latestEnergy.error ||
    latestMotion.error ||
    aggEnergy.error ||
    aggAir.error ||
    airWindow.error ||
    energyWindow.error;

  return (
    <div className="min-h-screen">

      {anyError ? (
        <div className="mx-auto max-w-[1600px] px-4 py-3 text-sm text-amber-300">
          Some requests completed with errors.{" "}
          <span className="text-slate-500">
            {String(
              latestAir.error?.message ??
                latestEnergy.error?.message ??
                airWindow.error?.message ??
                "",
            )}
          </span>
        </div>
      ) : null}

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
          <h2 className="mb-4 text-2xl font-semibold text-white"> Telemetry history ({rangeKey})</h2>

          <div className="flex flex-col gap-4">
            <AirCo2HistoryChart data={co2Series} legend={roomsFromAirHistory}/>

            <AirPm25HistoryChart data={pm25Series} legend={roomsFromAirHistory}/>

            <AirHumidityHistoryChart data={humiditySeries} legend={roomsFromAirHistory}/>

            <EnergyHistoryChart data={energySeries} legend={energyRooms}/>

            <AirPollutionHistoryChart data={groupedAirScatter}/>
          </div>
        </section>

        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white">Last values</h2>

          <div className="grid gap-4 lg:grid-cols-3">
            <LastAirQualityTable rows={airRows}/>

            <LastEnergyTable rows={energyRows}/>

            <LastMotionTable rows={motionRows}/>
          </div>
        </section>

        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white">Aggregates ({rangeKey})</h2>

          <EnergyAggregateTable rows={energyAggregatedRows}/>

          <div className="grid gap-4 lg:grid-cols-3">
            <Co2AggregateTable rows={airAggregatedRows}/>
            
            <Pm25AggregateTable rows={airAggregatedRows}/>
            
            <HumidityAggregateTable rows={airAggregatedRows}/>
          </div>
        </section>
      </main>
    </div>
  );
}


