import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../api.config';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TourItemInterface } from './tour-item/tour-item-interface';

// Communicates with the TourAPI to access the Database

@Injectable({
  providedIn: 'root',
})

export class TourService {
  private readonly baseTourUrl = `${API_BASE_URL}/tour`;

  constructor( private readonly http: HttpClient) {}

  // Read from query
  getAllTours(query? : string): Observable<TourItemInterface[]>{
    const params = query ? query : {};
    return this.http.get<TourItemInterface[]>(this.baseTourUrl, { params });
  }

  // Read tour by id
  getTour(id: string): Observable<TourItemInterface> {
    return this.http.get<TourItemInterface>(`${this.baseTourUrl}/${id}`);
  }

  // Create tour
  createTour(tour: TourItemInterface): Observable<string>{
    return this.http.post<string>(this.baseTourUrl,tour);
  }

  // Update tour
  updateTour(id:string, tour: TourItemInterface) : Observable<void>{
    return this.http.patch<void>(`${this.baseTourUrl}/${id}`,tour)
  }

  // Delete tour
  deleteTour(id: string): Observable<void>{
    return this.http.delete<void>(`${this.baseTourUrl}/${id}`);
  }

}
