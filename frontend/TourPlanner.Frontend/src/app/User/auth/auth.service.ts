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

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly authUrl = `${API_BASE_URL}/auth`;
  private readonly currentUserSubject = new BehaviorSubject<UserResponse | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private readonly http: HttpClient) {}

  get currentUser(): UserResponse | null {
    return this.currentUserSubject.value;
  }

  get isAuthenticated(): boolean {
    return this.currentUserSubject.value !== null;
  }

  register(request: RegisterUserRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.authUrl}/register`, request);
  }

  login(request: LoginUserRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.authUrl}/login`, request).pipe(
      tap((user) => this.currentUserSubject.next(user))
    );
  }

  logout(): void {
    this.currentUserSubject.next(null);
  }
}
