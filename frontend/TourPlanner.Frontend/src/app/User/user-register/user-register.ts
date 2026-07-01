import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { User } from '../user/user';

@Component({
  selector: 'app-user-register',
  imports: [RouterModule],
  templateUrl: './user-register.html',
  styleUrl: './user-register.css',
})
export class UserRegister {
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  newUser: User = {
    id: '',
    username: '',
    email: '',
    password: '',
  };

  checkpassword = '';

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  setEmail(value: string) {
    this.newUser.email = value;
  }

  setUsername(value: string) {
    this.newUser.username = value;
  }

  checkPassword(value: string) {
    this.checkpassword = value;
  }

  setPassword(value: string) {
    this.newUser.password = value;
  }

  get canRegister(): boolean {
    return (
      this.newUser.email.trim().length > 0 &&
      this.newUser.username.trim().length > 0 &&
      this.newUser.password.trim().length > 0 &&
      this.newUser.password === this.checkpassword &&
      !this.isLoading
    );
  }

  register(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.canRegister) {
      this.errorMessage = 'Please fill in all fields and make sure the passwords match.';
      return;
    }

    this.isLoading = true;

    this.authService
      .register({
        email: this.newUser.email,
        username: this.newUser.username,
        password: this.newUser.password,
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Registration successful.';
          this.router.navigate(['/login']);
        },
        error: (error) => {
          this.errorMessage =
            error.status === 409
              ? 'A user with this email or username already exists.'
              : 'Registration failed. Please try again.';
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        },
      });
  }
}
