import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TourItemInterface } from '../tour-item/tour-item-interface';

@Injectable({
  providedIn: 'root',
})


// Uses a BehaviourSubject to Store the Tours
// Subscribers get the value ot the Subject immediatly
// Updates on the BehaviourSubject will trigger the other subscribers to update theirs too
export class TourListService {
  private toursSubject = new BehaviorSubject<TourItemInterface[]>([]);
  tours$ = this.toursSubject.asObservable();

  get tours(): TourItemInterface[]{
    return this.toursSubject.value;
  }

  addTour(tour: TourItemInterface){
    // creates a new Array which has the new tour at the end
    const updated = [...this.toursSubject.value, tour];
    this.toursSubject.next(updated);
  }

  updateTour(originalTour: TourItemInterface, updatedTour: TourItemInterface): void {
    const updated = this.toursSubject.value.map(tour =>
      tour === originalTour ? updatedTour : tour
    );

    this.toursSubject.next(updated);
  }

  deleteTour(tourToDelete: TourItemInterface): void {
    const updated = this.toursSubject.value.filter(tour => tour !== tourToDelete);
    this.toursSubject.next(updated);
  }
}
