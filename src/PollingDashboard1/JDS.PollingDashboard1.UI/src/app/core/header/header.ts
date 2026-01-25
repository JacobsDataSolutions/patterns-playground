
import { Component, OnChanges, OnInit, SimpleChange, inject } from '@angular/core';
import { DEFAULT_THEME, THEMES } from '../../tokens';
import { usePreset } from '@primeuix/themes';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'jds-header',
  imports: [SelectModule, FormsModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit {
  selectedTheme: any = inject(DEFAULT_THEME);
  themes: any = inject(THEMES);

  ngOnInit(): void {
  }

  onChange(): void {
    usePreset(this.selectedTheme.preset);
    console.log(`Changed theme to ${this.selectedTheme.name}.`)
  }
}
