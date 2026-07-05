import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../api.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { TourItemInterface } from './interfaces/tour-interface/tour-item-interface';
import { TransportType } from './interfaces/tour-interface/transport-type';
import { CreateTourRequestInterface } from './interfaces/tour-dtos/create-tour-request-interface';
import { RouteInformation } from './interfaces/tour-dtos/route-information';

// Communicates with the TourAPI to access the Database


interface TourApiResponse {
  id: string;
  title: string
  description?: string;
  from: string;
  to: string;
  transportType: TransportType;
  route: RouteInformation;
}


export type UpdateTourRequestInterface = CreateTourRequestInterface;

@Injectable({
  providedIn: 'root',
})
export class TourService {

  private readonly baseTourUrl = `${API_BASE_URL}/tour`;

  constructor(private readonly http: HttpClient) { }

  // Read from query
  getAllTours(query?: string): Observable<TourItemInterface[]> {
    const params = new HttpParams().set('query', query ?? '');

    return this.http.get<TourApiResponse[]>(this.baseTourUrl, { params })
      .pipe(map((tours) => tours.map((tour) => this.toTour(tour))));
  }

  // Read tour by id
  getTour(id: string): Observable<TourItemInterface> {
    return this.http.get<TourApiResponse>(`${this.baseTourUrl}/${id}`)
    .pipe(map((tour)=> this.toTour(tour)));
  }

  // Create tour
  createTour(tour: TourItemInterface): Observable<TourItemInterface> {
    return this.http.post<TourApiResponse>(this.baseTourUrl, tour)
    .pipe(map((createdTour) => this.toTour(createdTour)));
  }

  // Update tour
  updateTour(id: string, tour: TourItemInterface): Observable<void> {
    return this.http.patch<void>(`${this.baseTourUrl}/${id}`, tour)
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
      route: response.route
    }
  }

}
