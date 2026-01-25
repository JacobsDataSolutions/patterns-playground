import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressButton } from './progress-button';

describe('ProgressButton', () => {
  let component: ProgressButton;
  let fixture: ComponentFixture<ProgressButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProgressButton]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProgressButton);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
