type ErrorPanelProps = {
  onRetry: () => void;
};

export function ErrorPanel({ onRetry }: ErrorPanelProps) {
  return (
    <div className="flex max-w-[620px] flex-col items-start gap-3.5 rounded border border-[#e3e1dc] border-t-4 border-t-[#c62828] bg-white p-10">
      <div className="text-[11px] font-bold tracking-[0.14em] text-[#c62828] uppercase">
        Erro de rede
      </div>
      <h2 className="text-2xl font-bold tracking-[-0.01em]">
        Não foi possível carregar os boletins
      </h2>
      <p className="max-w-[46ch] text-[15px] leading-relaxed text-[#4b504b] text-pretty">
        A consulta às três últimas semanas epidemiológicas falhou. Verifique
        a conexão e tente novamente — nenhum dado foi exibido para evitar
        leitura incorreta.
      </p>
      <button
        type="button"
        className="mt-1.5 cursor-pointer rounded-[3px] bg-[#141614] px-[22px] py-3 text-sm font-semibold text-white hover:bg-[#3a3d3a]"
        onClick={onRetry}
      >
        Tentar novamente
      </button>
    </div>
  );
}
