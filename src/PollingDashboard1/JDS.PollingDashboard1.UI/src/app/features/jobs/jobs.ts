import { Component, DestroyRef, OnInit, ViewChild, inject } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { JobsService } from './jobs-service';
import { Job } from './job';
import { AsyncPipe } from '@angular/common';
import { ProgressButton } from '../../shared/progress-button/progress-button';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RunningJob } from './running-job';
import { Observable, combineLatest, map, tap, timer } from 'rxjs';
import { TooltipModule } from 'primeng/tooltip';
import { formatDateTime } from '../../shared/util';

@Component({
  selector: 'jds-jobs',
  imports: [TableModule, AsyncPipe, ProgressButton, TooltipModule],
  templateUrl: './jobs.html',
  styleUrl: './jobs.scss',
})
export class Jobs implements OnInit {
  private readonly refreshWhenRunningListChanges = true;

  @ViewChild(Table) table!: Table<Job>;
  private readonly jobsService = inject(JobsService);
  private runningJobs = new Map<string, number>();
  private destroyRef = inject(DestroyRef);

  private now$ = timer(1000, 1000).pipe(
    takeUntilDestroyed(this.destroyRef),
    map(() => Date.now())
  );

  jobs$ = this.jobsService.jobs$

  formatDateTime = formatDateTime;

  tooltips$ = combineLatest([this.jobsService.runningJobs$, this.now$]).pipe(
    map(([running, now]) => {
      const m = new Map<string, string>();
      for (const job of running) {
        m.set(job.jobId, job.kickedOffUtc?.length ? this.formatElapsed(now - Date.parse(job.kickedOffUtc)) : '');
      }
      return m;
    })
  );

  get serverDateTime(): Observable<string> {
    return this.jobsService.serverLocalTime;
  }

  ngOnInit(): void {
    this.jobsService.startPolling();

    if (!this.refreshWhenRunningListChanges) {
      this.jobsService.refreshJobs()
        .pipe(
          takeUntilDestroyed(this.destroyRef)
        ).subscribe(() => {
          console.log('Jobs list loaded.');
        });
    }

    this.jobsService.runningJobs$
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        map(jobs => this.toRunningJobsMap(jobs))
      )
      .subscribe(map => {
        this.runningJobs = map;
        if (this.refreshWhenRunningListChanges) {
          this.jobsService.refreshJobs()
            .pipe(
              takeUntilDestroyed(this.destroyRef)
            ).subscribe(() => {
              console.log('Jobs list re-loaded.');
            });
        }
      });
  }

  buttonClicked(jobId: string): void {
    this.jobsService.runJob(jobId)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
      ).subscribe(job => {
        console.log(`Job ${job.id} kicked off.`);
      });
  }

  isRunning(jobId: string): boolean {
    return this.runningJobs.has(jobId);
  }

  kickedOffMs(jobId: string): number | null {
    return this.runningJobs.get(jobId) ?? null;
  }

  formatElapsed(ms: number): string {
    const seconds = Math.max(0, Math.floor(ms / 1000));
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
