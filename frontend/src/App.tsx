import { useCallback, useEffect, useMemo, useState } from "react";
import { AlertLegend } from "./components/AlertLegend";
import { ClosedWeeksNote } from "./components/ClosedWeeksNote";
import { ErrorPanel } from "./components/ErrorPanel";
import { LoadingState } from "./components/LoadingState";
import { WeekCard } from "./components/WeekCard";
import { WeekChart } from "./components/WeekChart";
import { WeekTable } from "./components/WeekTable";
import { fetchDengueWeek } from "./lib/api";
import { lastWeeks, toContractString, todayInSaoPaulo } from "./lib/epidemiologicalWeek";
import type { WeekSlot } from "./types/weekSlot";

type ScreenState = "loading" | "ok" | "error";

export default function App() {
  const [state, setState] = useState<ScreenState>("loading");
  const [weeks, setWeeks] = useState<WeekSlot[]>([]);
  const [reloadToken, setReloadToken] = useState(0);

  const load = useCallback(async () => {
    setState("loading");
    const targets = lastWeeks(todayInSaoPaulo(), 3);
    try {
      const results = await Promise.all(
        targets.map((target) => fetchDengueWeek(target.year, target.week)),
      );
      setWeeks(
        targets.map((target, index) => ({
          year: target.year,
          week: target.week,
          label: toContractString(target),
          data: results[index],
        })),
      );
      setState("ok");
    } catch {
      setWeeks([]);
      setState("error");
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load, reloadToken]);

  const chartData = useMemo(
    () =>
      [...weeks].reverse().map((slot) => ({
        semana: slot.label,
        casos_est: slot.data?.casos_est ?? null,
        casos_notificados: slot.data?.casos_notificados ?? null,
        vazio: slot.data === null,
      })),
    [weeks],
  );

  return (
    <div className="flex min-h-svh flex-col bg-[#f6f6f4] font-sans text-[#141614]">
      <header className="flex flex-wrap items-end justify-between gap-4 border-b-[3px] border-[#141614] bg-white px-5 py-5 md:px-10">
        <div className="flex flex-col gap-1.5">
          <div className="text-[11px] font-bold tracking-[0.16em] text-[#5c625c] uppercase">
            Alerta epidemiológico municipal
          </div>
          <h1 className="text-[28px] leading-none font-extrabold tracking-[-0.02em] md:text-[34px]">
            Dengue — Belo Horizonte
          </h1>
        </div>
        <div className="flex flex-col gap-1 text-right font-mono text-xs leading-relaxed text-[#5c625c]">
          <span>Boletim semanal · fonte Infodengue</span>
          <span>Três últimas semanas epidemiológicas fechadas</span>
        </div>
      </header>

      <main className="mx-auto flex w-full max-w-[1180px] flex-1 flex-col gap-8 px-5 py-8 md:px-10">
        {state !== "error" && <ClosedWeeksNote />}
        {state === "ok" && <AlertLegend />}

        {state === "error" && (
          <ErrorPanel onRetry={() => setReloadToken((token) => token + 1)} />
        )}

        {state === "loading" && <LoadingState />}

        {state === "ok" && (
          <div className="flex flex-col gap-8">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
              {weeks.map((slot) => (
                <WeekCard key={slot.label} slot={slot} />
              ))}
            </div>
            <WeekTable weeks={weeks} />
            <WeekChart data={chartData} />
          </div>
        )}
      </main>

      <footer className="flex flex-wrap justify-between gap-2 border-t border-[#e3e1dc] bg-white px-5 py-[18px] text-xs text-[#5c625c] md:px-10">
        <span>
          Dados estimados pelo Infodengue (Fiocruz/FGV). Casos notificados são
          registros do SINAN sujeitos a atraso.
        </span>
        <span className="font-mono">Semana epidemiológica inicia no domingo</span>
      </footer>
    </div>
  );
}
