import {
  Bar,
  BarChart,
  CartesianGrid,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { formatCases } from "../lib/alertLevel";
import type { ChartRow } from "../types/weekSlot";

export function WeekChart({ data }: { data: ChartRow[] }) {
  return (
    <section className="rounded border border-[#e3e1dc] bg-white p-6">
      <div className="mb-[26px] flex flex-wrap items-baseline justify-between gap-3">
        <h2 className="text-base font-bold tracking-[-0.01em]">Estimados × notificados</h2>
        <div className="flex gap-4">
          <span className="flex items-center gap-1.5 text-[13px] font-medium">
            <span className="block size-[13px] rounded-[2px] bg-[#0f4c5c]" />
            Casos estimados
          </span>
          <span className="flex items-center gap-1.5 text-[13px] font-medium">
            <span className="block size-[13px] rounded-[2px] bg-[#95bec9]" />
            Casos notificados
          </span>
        </div>
      </div>
      <div className="h-[280px] w-full">
        <ResponsiveContainer width="100%" height="100%">
          <BarChart data={data} barGap={8} barCategoryGap="28%">
            <CartesianGrid vertical={false} stroke="#edece8" strokeDasharray="4 4" />
            <XAxis
              dataKey="semana"
              tick={{ fill: "#4b504b", fontSize: 12, fontFamily: "IBM Plex Mono, ui-monospace, monospace" }}
              axisLine={{ stroke: "#d9d8d3" }}
              tickLine={false}
            />
            <YAxis
              tick={{ fill: "#797f79", fontSize: 11, fontFamily: "IBM Plex Mono, ui-monospace, monospace" }}
              axisLine={false}
              tickLine={false}
              allowDecimals={false}
            />
            <Tooltip
              formatter={(value, name) => [
                typeof value === "number" ? formatCases(value) : "Sem registro",
                name === "casos_est" ? "Estimados" : "Notificados",
              ]}
              labelStyle={{ fontFamily: "IBM Plex Mono, ui-monospace, monospace" }}
            />
            <Bar dataKey="casos_est" fill="#0f4c5c" radius={[2, 2, 0, 0]} maxBarSize={64} />
            <Bar dataKey="casos_notificados" fill="#95bec9" radius={[2, 2, 0, 0]} maxBarSize={64} />
          </BarChart>
        </ResponsiveContainer>
      </div>
    </section>
  );
}
