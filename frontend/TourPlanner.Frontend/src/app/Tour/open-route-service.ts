import { Injectable } from '@angular/core';
import { API_BASE_URL } from '../api.config';
import { HttpClient } from '@angular/common/http';
import { Coordinates } from './interfaces/tour-interface/coordinates';
import { Observable } from 'rxjs';
import { RouteInformation } from './interfaces/tour-dtos/route-information';
import { ORServiceRequestDto } from './interfaces/or-request-dto/or-request-dto';

@Injectable({
  providedIn: 'root',
})
export class OpenRouteService {

  private readonly baseORSUrl = `${API_BASE_URL}/ors`;

  constructor(private readonly http: HttpClient) { }

  GetCoordinatesOfPlace(value: string) : Observable<Coordinates>{
    return this.http.get<Coordinates>(`${this.baseORSUrl}/coordinates?location=${value}`);
  }

  GetRouteInformation(request: ORServiceRequestDto) : Observable<RouteInformation>{
    console.log(`Sending request to URL '${this.baseORSUrl}/route' with body ${JSON.stringify(request)}`,)
    return this.http.post<RouteInformation>(`${this.baseORSUrl}/route`,request);
  }
}
