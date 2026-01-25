import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
  selector: 'jds-progress-button',
  imports: [ButtonModule, ProgressSpinnerModule],
  templateUrl: './progress-button.html',
  styleUrl: './progress-button.scss',
})
export class ProgressButton {
  @Input() isRunning = false;
  @Output() buttonClicked: EventEmitter<any> = new EventEmitter();

  onClicked(): void {
    this.buttonClicked.emit();
  }
}
