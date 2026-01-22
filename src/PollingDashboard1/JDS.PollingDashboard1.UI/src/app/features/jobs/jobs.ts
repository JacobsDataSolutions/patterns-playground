import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { Table, TableModule } from 'primeng/table';
import { JobsService } from './jobs-service';
import { Job } from './job';
import { Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'jds-jobs',
  imports: [TableModule, AsyncPipe],
  templateUrl: './jobs.html',
  styleUrl: './jobs.scss',
})
export class Jobs implements OnInit {
  @ViewChild(Table) table!: Table<Job>;
  private readonly jobsService = inject(JobsService);

  jobs$!: Observable<Job[]>;

  ngOnInit(): void {
    this.jobs$ = this.jobsService.jobs$;
  }
}
