import { DestroyRef, Injectable, inject } from '@angular/core';
import { BaseComponent } from '../../core/base-component/base-component';
import { WEB_API_URL } from '../../tokens';
import { BehaviorSubject, EMPTY, Observable, catchError, ignoreElements, switchMap, tap, timer } from 'rxjs';
import { Job } from './job';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { RunningJobs } from './running-jobs';
import { RunningJob } from './running-job';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class JobsService extends BaseComponent {
  private readonly httpClient = inject(HttpClient);
  private readonly baseWebApiUrl = `${inject(WEB_API_URL)}/api`;
  private readonly destroyRef = inject(DestroyRef);

  private readonly runningJobsSubject$ = new BehaviorSubject<RunningJob[]>([]);

  private lastETag: string | null = null;
  private lastServerNowUtcMs: number | null = null;

  get jobs$(): Observable<Job[]> {
    return this.getAllJobs();
  }

  get runningJobs$(): Observable<RunningJob[]> {
    return this.runningJobsSubject$.asObservable();
  }

  get lastServerNowUtc(): number | null {
    return this.lastServerNowUtcMs;
  }

  runJob(jobId: string): Observable<void> {
    return this.httpClient.post<void>(`${this.baseWebApiUrl}/jobs/run/${encodeURIComponent(jobId)}`, {});
  }

  startPolling(intervalMs = 5000): void {
    timer(0, intervalMs)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        switchMap(() => this.fetchOnce())
    ).subscribe();
  }

  markRunningOptimistic(jobId: string, kickedOffUtcIsoString = new Date().toISOString()): void {
    const current = this.runningJobsSubject$.value;
    const next = current.some(j => j.jobId === jobId) ?
      current.map(j => j.jobId === jobId ? { ...j, kickedOffUtc: kickedOffUtcIsoString } : j) :
      [{ jobId, kickedOffUtc: kickedOffUtcIsoString }, ...current];
    this.runningJobsSubject$.next(next);
  }

  clearRunningOptimistic(jobId: string): void {
    const next = this.runningJobsSubject$.value.filter(j => j.jobId !== jobId);
    this.runningJobsSubject$.next(next);
  }

  private getAllJobs(): Observable<Job[]> {
    return this.httpClient.get<Job[]>(`${this.baseWebApiUrl}/jobs`);
  }

  private getRunningJobs(): Observable<HttpResponse<RunningJobs>> {
    const headers = this.lastETag ? { 'If-None-Match': this.lastETag } : undefined;
    return this.httpClient.get<RunningJobs>(`${this.baseWebApiUrl}/jobs/running`, { observe: 'response', headers });
  }

  private fetchOnce(): Observable<never> {
    return this.getRunningJobs()
      .pipe(
        tap((resp: HttpResponse<RunningJobs>) => {
          const eTag = resp.headers.get('ETag');
          if (eTag?.length) {
            this.lastETag = eTag;
          }
          if (resp.body?.serverTimeUtc?.length) {
            const ms = Date.parse(resp.body.serverTimeUtc);
            if (!Number.isNaN(ms)) {
              this.lastServerNowUtcMs = ms;
            }
          }
          if (resp.status === 304) {
            return;
          }
          const runningJobs = resp.body?.runningJobs?.length ? resp.body.runningJobs : [];
          this.runningJobsSubject$.next(runningJobs);
        }),
        ignoreElements(),
        catchError(err => {
          console.error('Querying running jobs failed!', err);
          return EMPTY;
        })
    );
  }
}
