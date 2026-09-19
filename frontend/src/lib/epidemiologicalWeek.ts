export type EpidemiologicalWeek = {
  year: number;
  week: number;
};

export function toContractString(week: EpidemiologicalWeek): string {
  return `${week.year}-${String(week.week).padStart(2, "0")}`;
}

function dayNumber(date: Date): number {
  return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()) / 86_400_000;
}

function atLocalDate(year: number, month: number, day: number): Date {
  return new Date(year, month - 1, day, 12, 0, 0);
}

export function todayInSaoPaulo(now = new Date()): Date {
  const parts = new Intl.DateTimeFormat("en-CA", {
    timeZone: "America/Sao_Paulo",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(now);

  const year = Number(parts.find((part) => part.type === "year")?.value);
  const month = Number(parts.find((part) => part.type === "month")?.value);
  const day = Number(parts.find((part) => part.type === "day")?.value);
  return atLocalDate(year, month, day);
}

export function startOfWeekSunday(date: Date): Date {
  const local = atLocalDate(date.getFullYear(), date.getMonth() + 1, date.getDate());
  local.setDate(local.getDate() - local.getDay());
  return local;
}

export function fromDate(date: Date): EpidemiologicalWeek {
  const year = date.getFullYear();
  const week1ThisYear = startOfWeekSunday(atLocalDate(year, 1, 4));
  const week1NextYear = startOfWeekSunday(atLocalDate(year + 1, 1, 4));
  if (dayNumber(date) < dayNumber(week1ThisYear)) {
    return fromDate(atLocalDate(year - 1, 12, 31));
  }
  if (dayNumber(date) >= dayNumber(week1NextYear)) {
    return { year: year + 1, week: 1 };
  }

  const sunday = startOfWeekSunday(date);
  const week = (dayNumber(sunday) - dayNumber(week1ThisYear)) / 7 + 1;
  return { year, week };
}

export function lastWeeks(now: Date, count = 3): EpidemiologicalWeek[] {
  const weeks: EpidemiologicalWeek[] = [];
  let cursorSunday = startOfWeekSunday(now);
  cursorSunday = new Date(cursorSunday);
  cursorSunday.setDate(cursorSunday.getDate() - 7);
  for (let i = 0; i < count; i++) {
    weeks.push(fromDate(cursorSunday));
    cursorSunday = new Date(cursorSunday);
    cursorSunday.setDate(cursorSunday.getDate() - 7);
  }
  return weeks;
}
