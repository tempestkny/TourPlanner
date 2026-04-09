import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-login',
  imports: [RouterModule],
  templateUrl: './user-login.html',
  styleUrl: './user-login.css',
})
export class UserLogin{
  identifier = '';
  password = '';
  errorMessage = '';

  constructor(private router:Router){}

  setIdentifier(value:string){
    this.identifier = value;
  }

  setPassword(value:string){
    this.password = value;
  }

  login() : boolean{
    console.log('Logging in with:',{
      username:this.identifier,
      password:this.password
    })
    return true;
  }

  onLoginClick(): void{
    if(this.login()){
      this.router.navigate(['/tour-list']);
    }else{
      this.errorMessage = 'Invalid username or password';
    }
  }
  
}
