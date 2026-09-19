export type DengueWeekResponse = {
  semana_epidemiologica: string;
  casos_est: number;
  casos_notificados: number;
  nivel_alerta: number;
};

function apiBaseUrl(): string {
  return import.meta.env.VITE_API_URL ?? "http://localhost:5080";
}

export async function fetchDengueWeek(
  year: number,
  week: number,
): Promise<DengueWeekResponse | null> {
  const response = await fetch(`${apiBaseUrl()}/api/dengue?ew=${week}&ey=${year}`);
  if (response.status === 404) {
    return null;
  }
  if (!response.ok) {
    throw new Error(`Falha ao consultar a semana ${year}-${String(week).padStart(2, "0")}.`);
  }
  return (await response.json()) as DengueWeekResponse;
}
