import { Routes } from '@angular/router';
import { Jobs } from './features/jobs/jobs';

export const routes: Routes = [
  {
    path: 'jobs',
    component: Jobs,
  },
  {
    path: '',
    component: Jobs,
    pathMatch: 'full'
  }
];
