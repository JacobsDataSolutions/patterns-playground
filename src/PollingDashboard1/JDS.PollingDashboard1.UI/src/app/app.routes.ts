import { Routes } from '@angular/router';
import { Jobs } from './features/jobs/jobs';

export const routes: Routes = [
  {
    path: '',
    component: Jobs,
    pathMatch: 'full'
  }
];
