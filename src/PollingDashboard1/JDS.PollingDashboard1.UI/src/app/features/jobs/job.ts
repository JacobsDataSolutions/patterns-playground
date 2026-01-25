export interface Job {
  id: string;
  name: string;
  number: number;
  lastRunUtc?: string;
  lastFinishedUtc?: string;
}
