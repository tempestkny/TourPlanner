import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { User } from "../user/user";


@Component({
  selector: 'app-user-register',
  imports: [RouterModule],
  templateUrl: './user-register.html',
  styleUrl: './user-register.css',
})
export class UserRegister {

  newUser: User = {
    id: "",
    username: "",
    email: "",
    password: ""
  };
  checkpassword: string = '';

  setEmail(arg0: string) {
    this.newUser.email = arg0;
  }

  setUsername(arg0: string) {
    this.newUser.username = arg0;
  }

  checkPassword(arg0: string) {
    this.checkpassword = arg0;
  }
  setPassword(arg0: string) {
    this.newUser.password = arg0;
  }

  get canRegister(): boolean {
    return (
      this.newUser.email.trim().length > 0 &&
      this.newUser.username.trim().length > 0 &&
      this.newUser.password.trim().length > 0 &&
      this.newUser.password === this.checkpassword
    );
  }

  setUserID() {
    this.newUser.id = crypto.randomUUID();
  }


  register(): void {
    if (this.canRegister) {
      this.setUserID();
      console.log('Registering:', {
        id: this.newUser.id,
        email: this.newUser.email,
        username: this.newUser.username,
        password: this.newUser.password
      })
    }
    else
      console.log('Password Check failed:', {
        password: this.newUser.password,
        checkpassword: this.checkpassword
      })
  }







}
