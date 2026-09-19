import { alertThemeOrUnknown, formatCases } from "../lib/alertLevel";
import type { WeekSlot } from "../types/weekSlot";

export function WeekCard({ slot }: { slot: WeekSlot }) {
  if (slot.data === null) {
    return (
      <article
        className="flex min-h-[206px] flex-col justify-between rounded p-[22px_24px]"
        style={{
          background: "#f1f0ec",
          color: "#5c625c",
          border: "1px dashed #c9c7c0",
        }}
      >
        <div className="flex items-start justify-between gap-3">
          <div className="font-mono text-[15px] font-semibold tracking-[0.02em]">{slot.label}</div>
          <div
            className="rounded-[2px] px-2.5 py-1 text-[11px] font-bold tracking-[0.12em] uppercase"
            style={{ background: "#e3e1dc", color: "#5c625c" }}
          >
            Nível —
          </div>
        </div>
        <div className="flex flex-col gap-2">
          <div className="text-[28px] font-bold tracking-[-0.01em] text-[#5c625c]">
            Sem registro
          </div>
          <p className="max-w-[30ch] text-[13px] leading-snug text-[#5c625c]">
            Boletim ainda não publicado para esta semana epidemiológica.
          </p>
        </div>
      </article>
    );
  }

  const theme = alertThemeOrUnknown(slot.data.nivel_alerta);

  return (
    <article
      className="flex min-h-[206px] flex-col justify-between rounded p-[22px_24px]"
      style={{ background: theme.fundo, color: theme.tinta }}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="font-mono text-[15px] font-semibold tracking-[0.02em]">{slot.label}</div>
        <div
          className="rounded-[2px] px-2.5 py-1 text-[11px] font-bold tracking-[0.12em] uppercase"
          style={{ background: theme.selo, color: theme.tinta }}
        >
          {`Nível ${slot.data.nivel_alerta} · ${theme.nome}`}
        </div>
      </div>
      <div className="flex flex-col gap-3.5">
        <div>
          <div
            className="text-xs font-semibold tracking-[0.1em] uppercase"
            style={{ color: theme.fraca }}
          >
            Casos estimados
          </div>
          <div className="text-[52px] leading-[1.05] font-extrabold tracking-[-0.03em] tabular-nums">
            {formatCases(slot.data.casos_est)}
          </div>
        </div>
        <div
          className="flex items-baseline gap-2 border-t pt-3"
          style={{ borderColor: theme.linha }}
        >
          <span
            className="text-xs font-semibold tracking-[0.1em] uppercase"
            style={{ color: theme.fraca }}
          >
            Notificados
          </span>
          <span className="ml-auto text-[22px] font-bold tabular-nums">
            {formatCases(slot.data.casos_notificados)}
          </span>
        </div>
      </div>
    </article>
  );
}
