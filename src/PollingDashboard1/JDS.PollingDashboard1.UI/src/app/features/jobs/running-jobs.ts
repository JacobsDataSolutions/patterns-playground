import { RunningJob } from "./running-job";

export interface RunningJobs {
  runningJobs: RunningJob[];
  serverTimeUtc: string;
}
