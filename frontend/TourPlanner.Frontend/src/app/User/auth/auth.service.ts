import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../api.config';

export interface RegisterUserRequest {
  email: string;
  username: string;
  password: string;
}

export interface LoginUserRequest {
  identifier: string;
  password: string;
}

export interface UserResponse {
  id: string;
  email: string;
  username: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly authUrl = `${API_BASE_URL}/auth`;

  constructor(private readonly http: HttpClient) {}

  register(request: RegisterUserRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.authUrl}/register`, request);
  }

  login(request: LoginUserRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.authUrl}/login`, request);
  }
}
