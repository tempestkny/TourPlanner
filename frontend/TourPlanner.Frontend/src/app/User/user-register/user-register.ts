import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";


@Component({
  selector: 'app-user-register',
  imports: [RouterModule],
  templateUrl: './user-register.html',
  styleUrl: './user-register.css',
})
export class UserRegister {

  email: string = '';
  username: string = '';
  password: string = '';
  checkpassword: string = '';


  setEmail(arg0: string) {
    this.email = arg0;
  }

  setUsername(arg0: string) {
    this.username = arg0;
  }

  checkPassword(arg0: string) {
    this.checkpassword = arg0;
  }
  setPassword(arg0: string) {
    this.password = arg0;
  }

  get canRegister(): boolean {
    return (
      this.email.trim().length > 0 &&
      this.username.trim().length > 0 &&
      this.password.trim().length > 0 &&
      this.password === this.checkpassword
    );
  }


  register(): void {
    if (this.password === this.checkpassword)
      console.log('Registering:', {
        email: this.email,
        username: this.username,
        password: this.password
      })
    else
      console.log('Password Check failed:', {
        password: this.password,
        checkpassword: this.checkpassword
      })
  }







}
