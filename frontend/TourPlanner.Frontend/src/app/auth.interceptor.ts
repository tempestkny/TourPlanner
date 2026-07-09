import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './User/auth/auth.service';

// Adds User Token to every Request, so that the user can access the tour functions

export const authInterceptor: HttpInterceptorFn = (req, next) => { // Runs for every request made to the backend
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.accessToken; // reads token from local storage
  const authReq = token // if token exists, request is cloned and this header added
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      })
    : req;

  return next(authReq).pipe( // if unauthorized, user is logged out and redirected to login page
    catchError((error: HttpErrorResponse) => {
      
      if (error.status === 401) {
        authService.logout();
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
