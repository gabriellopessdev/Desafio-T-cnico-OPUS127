export function LoadingState() {
  return (
    <div className="flex flex-col gap-8">
      <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
        <div className="h-[206px] animate-[pulso-skeleton_1.4s_ease-in-out_infinite] rounded bg-[#e2e3e0]" />
        <div className="h-[206px] animate-[pulso-skeleton_1.4s_ease-in-out_0.18s_infinite] rounded bg-[#e2e3e0]" />
        <div className="h-[206px] animate-[pulso-skeleton_1.4s_ease-in-out_0.36s_infinite] rounded bg-[#e2e3e0]" />
      </div>
      <div className="flex items-center gap-2.5 font-mono text-xs text-[#5c625c]">
        <span className="block size-[11px] animate-[giro_0.9s_linear_infinite] rounded-full border-2 border-[#c9ccc7] border-t-[#141614]" />
        Carregando 3 semanas epidemiológicas…
      </div>
      <div className="flex flex-col gap-3.5 rounded border border-[#e3e1dc] bg-white p-[22px]">
        <div className="h-3.5 w-[180px] animate-[pulso-skeleton_1.4s_ease-in-out_infinite] rounded-[2px] bg-[#e2e3e0]" />
        <div className="h-3 w-full rounded-[2px] bg-[#eceeea]" />
        <div className="h-3 w-full rounded-[2px] bg-[#eceeea]" />
        <div className="h-3 w-full rounded-[2px] bg-[#eceeea]" />
      </div>
      <div className="flex h-[300px] items-end gap-[26px] rounded border border-[#e3e1dc] bg-white p-7">
        <div className="h-[38%] flex-1 animate-[pulso-skeleton_1.4s_ease-in-out_infinite] rounded-t-[2px] bg-[#e2e3e0]" />
        <div className="h-[62%] flex-1 rounded-t-[2px] bg-[#eceeea]" />
        <div className="h-[48%] flex-1 animate-[pulso-skeleton_1.4s_ease-in-out_0.2s_infinite] rounded-t-[2px] bg-[#e2e3e0]" />
        <div className="h-[80%] flex-1 rounded-t-[2px] bg-[#eceeea]" />
      </div>
    </div>
  );
}
