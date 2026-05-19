import { useEffect, useMemo, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { SIGNALR_URL } from "../config";
import { TelemetryBatchEvent } from "../types/data";

export function useSignalRTelemetry() {
  const [status, setStatus] = useState("disconnected");
  const [lastEvent, setLastEvent] = useState<TelemetryBatchEvent | null>(null);
  const [error, setError] = useState<unknown>(null);

  const connection = useMemo(
    () =>
      new signalR.HubConnectionBuilder()
        .withUrl(SIGNALR_URL)
        .withAutomaticReconnect()
        .build(),
    []
  );

  useEffect(() => {
    let mounted = true;
    connection.on("telemetryBroadcaster", (evt: TelemetryBatchEvent) => {
      if (mounted) {
        setLastEvent(evt);
      }
    });

    void (async () => {
      try {
        setStatus("connecting");
        await connection.start();
        if (mounted) setStatus("connected");
      } catch (e) {
        if (mounted) {
          setStatus("error");
          setError(e);
        }
      }
    })();

    return () => {
      mounted = false;
      connection.off("telemetryBroadcaster");
      void connection.stop();
    };
  }, [connection]);

  return { status, lastEvent, error };
}
