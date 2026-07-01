import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-user-login',
  imports: [RouterModule],
  templateUrl: './user-login.html',
  styleUrl: './user-login.css',
})
export class UserLogin {
  identifier = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  setIdentifier(value: string) {
    this.identifier = value;
  }

  setPassword(value: string) {
    this.password = value;
  }

  get canLogin(): boolean {
    return (
      this.identifier.trim().length > 0 &&
      this.password.trim().length > 0 &&
      !this.isLoading
    );
  }

  onLoginClick(): void {
    this.errorMessage = '';

    if (!this.canLogin) {
      this.errorMessage = 'Please enter username/email and password.';
      return;
    }

    this.isLoading = true;

    this.authService
      .login({
        identifier: this.identifier,
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.router.navigate(['/tours']);
        },
        error: (error: HttpErrorResponse) => {
          this.errorMessage =
            error.status === 401
              ? 'Invalid username/email or password.'
              : 'Login failed. Please try again.';
          this.isLoading = false;
          this.changeDetector.detectChanges();
        },
        complete: () => {
          this.isLoading = false;
          this.changeDetector.detectChanges();
        },
      });
  }
}
