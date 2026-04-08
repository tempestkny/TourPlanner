import { Injectable, signal } from '@angular/core';
import { TourItemInterface } from './tour-item-interface';

@Injectable({
  providedIn: 'root',
})
export class TourItemService {
  private _tour = signal<TourItemInterface | null>(null);
  tour = this._tour.asReadonly();

  setTour(t: TourItemInterface) {
    this._tour.set(t);
  }

  updateTour(partial: Partial<TourItemInterface>) {
    this._tour.update(old => old ? { ...old, ...partial } : old);
  }

  deleteTour() {
    this._tour.set(null);
  }

  getTour(): TourItemInterface | null {
    return this._tour();
  }
}
