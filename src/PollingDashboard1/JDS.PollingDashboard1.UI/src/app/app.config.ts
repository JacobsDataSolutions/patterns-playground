import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';
import { environment } from '../environments/environment';
import { DEFAULT_THEME, THEMES, WEB_API_URL } from './tokens';
import { providePrimeNG } from 'primeng/config';

import Aura from '@primeuix/themes/aura';
import Lara from '@primeuix/themes/lara';
import Material from '@primeuix/themes/material';
import Nora from '@primeuix/themes/nora';
import { getRandomInt } from './shared/util';

const themes = [
  { name: 'Aura', preset: Aura },
  { name: 'Lara', preset: Lara },
  { name: 'Material', preset: Material },
  { name: 'Nora', preset: Nora }
];

const theme = themes[getRandomInt(themes.length)];

export const appConfig: ApplicationConfig = {
  providers: [
    provideZonelessChangeDetection(),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    providePrimeNG({
      theme: {
        preset: theme.preset
      }
    }),
    {
      provide: WEB_API_URL,
      useValue: environment.webApiUrl
    },
    {
      provide: THEMES,
      useValue: themes
    },
    {
      provide: DEFAULT_THEME,
      useValue: theme
    }
  ]
};
