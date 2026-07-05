import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TourService, UpdateTourRequestInterface } from '../tour-service';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import { Coordinates } from '../interfaces/tour-interface/coordinates';
import { OpenRouteService } from '../open-route-service';
import { CreateTourRequestInterface } from '../interfaces/tour-dtos/create-tour-request-interface';

@Injectable({
  providedIn: 'root',
})


// Uses a BehaviourSubject to Store the Tours
// Subscribers get the value ot the Subject immediatly
// Updates on the BehaviourSubject will trigger the other subscribers to update theirs too
export class TourListService {
  private toursSubject = new BehaviorSubject<TourItemInterface[]>([]);
  tours$ = this.toursSubject.asObservable();

  query: string = '';

  constructor(private tourService: TourService) { }

  loadTours(): void {
    this.tourService.getAllTours(this.query).subscribe({
      next: (tours) => this.toursSubject.next(tours), // update when successfull
      error: (err) => console.error('Failed to load tours: ', err) // throw error when not
    })
  }

  addTour(tour: TourItemInterface) {
    this.tourService.createTour(tour).subscribe({
      next: () => {
        this.loadTours();
      },
      error: (err) => console.error('Failed to create tour: ', err)
    })
  }

  updateTour(id: string, newTour: TourItemInterface): void {
    this.tourService.updateTour(id, newTour).subscribe({
      next: () => {
        const updatedTour = this.toursSubject.value.map(tour =>
          tour.id === id ? newTour : tour
        );
        this.loadTours();
      },
      error: (err) => console.error('Failed to create tour: ', err)
    })
  }

  deleteTour(id: string): void {
    this.tourService.deleteTour(id).subscribe({
      next: () => {
        const newList = this.toursSubject.value.filter(tour => tour.id !== id);
        this.toursSubject.next(newList);
        this.loadTours();
      },
      error: (err) => console.error('Failed to delete tour: ', err)
    })
  }
}
