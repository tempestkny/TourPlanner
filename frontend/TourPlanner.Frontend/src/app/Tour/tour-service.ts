import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../api.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { TourItemInterface } from './tour-item/tour-item-interface';

// Communicates with the TourAPI to access the Database

interface TourApiResponse {
  id: string;
  title: string
  description?: string;
  from: string;
  to: string;
  transportType: string;

  distance?: string;
  time?: string;
}

export type CreateTourRequest = Omit<TourItemInterface, 'id'|'distance'|'time'>;
export type UpdateTourRequest = Omit<TourItemInterface, 'id'|'distance'|'time'>;

@Injectable({
  providedIn: 'root',
})

export class TourService {
  private readonly baseTourUrl = `${API_BASE_URL}/tour`;

  constructor(private readonly http: HttpClient) { }

  // Read from query
  getAllTours(query?: string): Observable<TourItemInterface[]> {
    const params = new HttpParams().set('query', query ?? '');

    return this.http.get<TourItemInterface[]>(this.baseTourUrl, { params })
      .pipe(map((tours) => tours.map((tour) => this.toTour(tour))));
  }

  // Read tour by id
  getTour(id: string): Observable<TourItemInterface> {
    return this.http.get<TourItemInterface>(`${this.baseTourUrl}/${id}`)
    .pipe(map((tour)=> this.toTour(tour)));
  }

  // Create tour
  createTour(tour: CreateTourRequest): Observable<TourItemInterface> {
    return this.http.post<TourApiResponse>(this.baseTourUrl, tour)
    .pipe(map((createdTour) => this.toTour(createdTour)));
  }

  // Update tour
  updateTour(id: string, tour: TourItemInterface): Observable<void> {
    return this.http.patch<void>(`${this.baseTourUrl}/${id}`, this.toApiRequest(tour))
  }

  // Delete tour
  deleteTour(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseTourUrl}/${id}`);
  }


  private toTour(response: TourApiResponse):TourItemInterface{
    return{
      id: response.id,
      title: response.title,
      description: response.description,
      from: response.from,
      to: response.to,
      transportType: response.transportType,

      time: response.time,
      distance: response.distance
    }
  }
  private toApiRequest(tour:CreateTourRequest | UpdateTourRequest){
    return{
      ...tour,
    }
  }

}
