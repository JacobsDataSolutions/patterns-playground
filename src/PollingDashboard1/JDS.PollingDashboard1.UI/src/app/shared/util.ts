export function getRandomInt(max: number): number {
  return Math.floor(Math.random() * max);
}

export function formatDateTime(value: any): string {
  if (!value) {
    return '';
  }
  const d = value instanceof Date ? value : new Date(value);

  const yyyy = d.getFullYear();
  const MM = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  const HH = String(d.getHours()).padStart(2, '0');
  const mm = String(d.getMinutes()).padStart(2, '0');
  const ss = String(d.getSeconds()).padStart(2, '0');

  return `${yyyy}-${MM}-${dd} ${HH}:${mm}:${ss}`;
}
