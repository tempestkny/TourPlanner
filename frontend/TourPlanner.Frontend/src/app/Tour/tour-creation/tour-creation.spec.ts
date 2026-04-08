import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourCreation } from './tour-creation';

describe('TourCreation', () => {
  let component: TourCreation;
  let fixture: ComponentFixture<TourCreation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourCreation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TourCreation);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
