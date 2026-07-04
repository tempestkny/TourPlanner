import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_BASE_URL } from '../../api.config';
import { TourLogInterface } from './tour-log-interface';

interface TourLogApiResponse {
  id: string;
  tourId: string;
  timeStamp: string;
  comment?: string;
  difficulty?: TourLogInterface['difficulty'];
  totalDistance?: number;
  totalTime?: number;
  rating?: number;
}

export type CreateTourLogRequest = Omit<TourLogInterface, 'id'>;
export type UpdateTourLogRequest = Omit<TourLogInterface, 'id' | 'tourId'>;

@Injectable({
  providedIn: 'root',
})
export class TourLogService {
  private readonly baseTourLogUrl = `${API_BASE_URL}/tourlogs`;

  constructor(private readonly http: HttpClient) {}

  getLogsByTourId(tourId: string): Observable<TourLogInterface[]> {
    return this.http
      .get<TourLogApiResponse[]>(`${this.baseTourLogUrl}/tour/${tourId}`)
      .pipe(map((logs) => logs.map((log) => this.toTourLog(log))));
  }

  getLog(id: string): Observable<TourLogInterface> {
    return this.http
      .get<TourLogApiResponse>(`${this.baseTourLogUrl}/${id}`)
      .pipe(map((log) => this.toTourLog(log)));
  }

  createLog(log: CreateTourLogRequest): Observable<TourLogInterface> {
    return this.http
      .post<TourLogApiResponse>(this.baseTourLogUrl, this.toApiRequest(log))
      .pipe(map((createdLog) => this.toTourLog(createdLog)));
  }

  updateLog(id: string, log: UpdateTourLogRequest): Observable<void> {
    return this.http.put<void>(`${this.baseTourLogUrl}/${id}`, this.toApiRequest(log));
  }

  deleteLog(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseTourLogUrl}/${id}`);
  }

  private toTourLog(response: TourLogApiResponse): TourLogInterface {
    return {
      id: response.id,
      tourId: response.tourId,
      timeStamp: new Date(response.timeStamp),
      comment: response.comment,
      difficulty: response.difficulty,
      totalDistance: response.totalDistance,
      totalTime: response.totalTime,
      rating: response.rating,
    };
  }

  private toApiRequest(log: CreateTourLogRequest | UpdateTourLogRequest) {
    return {
      ...log,
      timeStamp: log.timeStamp.toISOString(),
    };
  }
}
