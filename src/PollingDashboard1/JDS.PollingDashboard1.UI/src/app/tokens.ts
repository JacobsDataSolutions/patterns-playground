import { InjectionToken } from "@angular/core";

export const WEB_API_URL = new InjectionToken<string>('WebApiUrl');

export const THEMES = new InjectionToken<any>('Themes');

export const DEFAULT_THEME = new InjectionToken<any>('DefaultTheme');
