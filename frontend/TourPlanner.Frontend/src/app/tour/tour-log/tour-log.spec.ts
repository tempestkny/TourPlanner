import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourLog } from './tour-log';

describe('TourLog', () => {
  let component: TourLog;
  let fixture: ComponentFixture<TourLog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourLog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TourLog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
