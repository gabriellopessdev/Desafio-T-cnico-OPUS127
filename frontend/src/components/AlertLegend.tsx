export function AlertLegend() {
  return (
    <div className="flex flex-wrap items-center gap-[18px] pb-1">
      <span className="text-[11px] font-bold tracking-[0.14em] text-[#5c625c] uppercase">
        Nível de alerta
      </span>
      <div className="flex flex-wrap gap-3.5">
        <LegendSwatch color="#18794e" label="1 · Baixo" />
        <LegendSwatch color="#f2c200" label="2 · Atenção" />
        <LegendSwatch color="#e8702a" label="3 · Alto" />
        <LegendSwatch color="#c62828" label="4 · Muito alto" />
      </div>
    </div>
  );
}

function LegendSwatch({ color, label }: { color: string; label: string }) {
  return (
    <div className="flex items-center gap-1.5">
      <span className="block size-[13px] rounded-[2px]" style={{ background: color }} />
      <span className="text-[13px] font-medium">{label}</span>
    </div>
  );
}
