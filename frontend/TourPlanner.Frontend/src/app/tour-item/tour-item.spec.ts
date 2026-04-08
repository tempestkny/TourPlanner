import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourItemComponent } from './tour-item.component';

describe('TourItem', () => {
  let component: TourItemComponent;
  let fixture: ComponentFixture<TourItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TourItemComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
