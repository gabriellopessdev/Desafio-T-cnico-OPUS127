import { alertThemeOrUnknown, formatCases } from "../lib/alertLevel";
import type { WeekSlot } from "../types/weekSlot";

export function WeekTable({ weeks }: { weeks: WeekSlot[] }) {
  return (
    <section className="overflow-hidden rounded border border-[#e3e1dc] bg-white">
      <div className="flex items-baseline justify-between gap-3 border-b border-[#e3e1dc] px-6 py-[18px]">
        <h2 className="text-base font-bold tracking-[-0.01em]">Detalhamento por semana</h2>
        <span className="font-mono text-[11px] text-[#797f79]">
          casos_est · casos_notificados · nivel_alerta
        </span>
      </div>
      <div className="overflow-x-auto">
        <table className="w-full border-collapse">
          <thead>
            <tr className="bg-[#fafaf8]">
              <th className="border-b border-[#e3e1dc] px-6 py-3 text-left text-[11px] font-bold tracking-[0.12em] text-[#5c625c] uppercase">
                Semana
              </th>
              <th className="border-b border-[#e3e1dc] px-6 py-3 text-right text-[11px] font-bold tracking-[0.12em] text-[#5c625c] uppercase">
                Estimados
              </th>
              <th className="border-b border-[#e3e1dc] px-6 py-3 text-right text-[11px] font-bold tracking-[0.12em] text-[#5c625c] uppercase">
                Notificados
              </th>
              <th className="border-b border-[#e3e1dc] px-6 py-3 text-left text-[11px] font-bold tracking-[0.12em] text-[#5c625c] uppercase">
                Nível
              </th>
            </tr>
          </thead>
          <tbody>
            {weeks.map((slot) => {
              const empty = slot.data === null;
              const theme = slot.data ? alertThemeOrUnknown(slot.data.nivel_alerta) : null;
              return (
                <tr key={slot.label}>
                  <td className="border-b border-[#f0efeb] px-6 py-3.5 font-mono text-sm font-medium">
                    {slot.label}
                  </td>
                  <td
                    className="border-b border-[#f0efeb] px-6 py-3.5 text-right text-[15px] font-semibold tabular-nums"
                    style={{ color: empty ? "#8b918b" : "#141614" }}
                  >
                    {empty ? "—" : formatCases(slot.data!.casos_est)}
                  </td>
                  <td
                    className="border-b border-[#f0efeb] px-6 py-3.5 text-right text-[15px] font-semibold tabular-nums"
                    style={{ color: empty ? "#8b918b" : "#141614" }}
                  >
                    {empty ? "—" : formatCases(slot.data!.casos_notificados)}
                  </td>
                  <td className="border-b border-[#f0efeb] px-6 py-3.5">
                    <span
                      className="inline-flex items-center gap-2 text-sm font-semibold"
                      style={{ color: empty ? "#8b918b" : "#141614" }}
                    >
                      <span
                        className="block size-3 rounded-[2px]"
                        style={{ background: empty ? "#d3d2cd" : theme!.fundo }}
                      />
                      {empty
                        ? "Sem registro"
                        : `${slot.data!.nivel_alerta} · ${theme!.nome}`}
                    </span>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </section>
  );
}
