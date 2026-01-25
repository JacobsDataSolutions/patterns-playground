import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { JobsService } from './jobs-service';
import { Job } from './job';
import { AsyncPipe } from '@angular/common';
import { ProgressButton } from '../../shared/progress-button/progress-button';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RunningJob } from './running-job';
import { map } from 'rxjs';

@Component({
  selector: 'jds-jobs',
  imports: [TableModule, AsyncPipe, ProgressButton],
  templateUrl: './jobs.html',
  styleUrl: './jobs.scss',
})
export class Jobs implements OnInit {
  @ViewChild(Table) table!: Table<Job>;
  private readonly jobsService = inject(JobsService);
  private runningJobs = new Map<string, number>();

  jobs$ = this.jobsService.jobs$

  ngOnInit(): void {
    this.jobsService.refreshJobs().subscribe(() => {
      console.log('Jobs list loaded.');
      this.jobsService.startPolling();
    });
    this.jobsService.runningJobs$
      .pipe(
        takeUntilDestroyed(),
        map(jobs => this.toRunningJobsMap(jobs))
      )
      .subscribe(map => {
        this.runningJobs = map;
      });
  }

  buttonClicked(jobId: string): void {
    this.jobsService.runJob(jobId);
  }

  isRunning(jobId: string): boolean {
    return this.runningJobs.has(jobId);
  }

  kickedOffMs(jobId: string): number | null {
    return this.runningJobs.get(jobId) ?? null;
  }

  formatElaspsed(jobId: string): string {
    const kicked = this.kickedOffMs(jobId);
    if (!kicked) {
      return '';
    }
    const now = Date.now();
    const seconds = Math.max(0, Math.floor((now - kicked) / 1000));
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return 'Running for ' + (m > 0 ? `${m}m ${s}s` : `${s}s`);
  }

  private toRunningJobsMap(jobs: RunningJob[]): Map<string, number> {
    const map = new Map<string, number>();
    for (const j of jobs) {
      const ms = Date.parse(j.kickedOffUtc);
      if (!Number.isNaN(ms)) {
        map.set(j.jobId, ms);
      }
    }
    return map;
  }
}
