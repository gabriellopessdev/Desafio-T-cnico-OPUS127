import type { DengueWeekResponse } from "../lib/api";

export type WeekSlot = {
  year: number;
  week: number;
  label: string;
  data: DengueWeekResponse | null;
};

export type ChartRow = {
  semana: string;
  casos_est: number | null;
  casos_notificados: number | null;
  vazio: boolean;
};
