
import { Component, inject } from '@angular/core';
import { DEFAULT_THEME, THEMES } from '../../tokens';
import { usePreset } from '@primeuix/themes';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'jds-header',
  imports: [SelectModule, FormsModule, AsyncPipe],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  selectedTheme: any = inject(DEFAULT_THEME);
  themes: any = inject(THEMES);

  onChange(): void {
    usePreset(this.selectedTheme.preset);
    console.log(`Changed theme to ${this.selectedTheme.name}.`)
  }
}
