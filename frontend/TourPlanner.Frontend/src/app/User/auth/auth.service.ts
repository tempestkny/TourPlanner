import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
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

export interface LoginResponse extends UserResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenStorageKey = 'tourplanner.auth.token';
  private readonly authUrl = `${API_BASE_URL}/auth`;
  private readonly currentUserSubject = new BehaviorSubject<UserResponse | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private readonly http: HttpClient) {}

  get currentUser(): UserResponse | null {
    return this.currentUserSubject.value;
  }

  get isAuthenticated(): boolean {
    return this.accessToken !== null;
  }

  get accessToken(): string | null {
    return localStorage.getItem(this.tokenStorageKey);
  }

  register(request: RegisterUserRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.authUrl}/register`, request);
  }

  login(request: LoginUserRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.authUrl}/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenStorageKey, response.token);
        this.currentUserSubject.next({
          id: response.id,
          email: response.email,
          username: response.username,
        });
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenStorageKey);
    this.currentUserSubject.next(null);
  }
}
