import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api.config';

export interface Statistics {
  totalTours: number;
  totalTourLogs: number;
  totalDistance: number;
  totalTime: number;
  averageRating: number;
}

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  private readonly statisticsUrl = `${API_BASE_URL}/statistics`;

  constructor(private readonly http: HttpClient) {}

  getStatistics(): Observable<Statistics> { // jwt token is added by the interceptor
    return this.http.get<Statistics>(this.statisticsUrl);
  }
}
