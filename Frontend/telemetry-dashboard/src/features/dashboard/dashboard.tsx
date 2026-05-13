
export default function Dashboard() {
  
  return (
    <div className="min-h-screen">

      <main className="mx-auto space-y-10 px-4 py-8">
        <section>
          <h2 className="mb-4 text-2xl font-semibold text-white">Current metrics</h2>
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


