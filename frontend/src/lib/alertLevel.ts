export type AlertLevel = 1 | 2 | 3 | 4;

export type AlertTheme = {
  nome: string;
  fundo: string;
  tinta: string;
  fraca: string;
  linha: string;
  selo: string;
};

const NIVEIS: Record<AlertLevel, AlertTheme> = {
  1: {
    nome: "Baixo",
    fundo: "#18794e",
    tinta: "#ffffff",
    fraca: "#ffffff",
    linha: "rgba(255,255,255,.28)",
    selo: "rgba(255,255,255,.18)",
  },
  2: {
    nome: "Atenção",
    fundo: "#f2c200",
    tinta: "#241c00",
    fraca: "#241c00",
    linha: "rgba(36,28,0,.22)",
    selo: "rgba(36,28,0,.12)",
  },
  3: {
    nome: "Alto",
    fundo: "#e8702a",
    tinta: "#ffffff",
    fraca: "#ffffff",
    linha: "rgba(255,255,255,.3)",
    selo: "rgba(255,255,255,.2)",
  },
  4: {
    nome: "Muito alto",
    fundo: "#c62828",
    tinta: "#ffffff",
    fraca: "#ffffff",
    linha: "rgba(255,255,255,.3)",
    selo: "rgba(255,255,255,.2)",
  },
};

export function isAlertLevel(value: number): value is AlertLevel {
  return value === 1 || value === 2 || value === 3 || value === 4;
}

export const unknownAlertTheme: AlertTheme = {
  nome: "Indefinido",
  fundo: "#3a3d3a",
  tinta: "#ffffff",
  fraca: "#ffffff",
  linha: "rgba(255,255,255,.28)",
  selo: "rgba(255,255,255,.18)",
};

export function alertTheme(level: number): AlertTheme | null {
  return isAlertLevel(level) ? NIVEIS[level] : null;
}

export function alertThemeOrUnknown(level: number): AlertTheme {
  return alertTheme(level) ?? unknownAlertTheme;
}

export function formatCases(value: number): string {
  return value.toLocaleString("pt-BR", { maximumFractionDigits: 2 });
}
