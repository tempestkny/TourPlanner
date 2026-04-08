import { Component } from '@angular/core';
import {User} from '../user/user'

@Component({
  selector: 'app-user-login',
  imports: [],
  templateUrl: './user-login.html',
  styleUrl: './user-login.css',
})
export class UserLogin{
  identifier = '';
  password = '';

  setIdentifier(value:string){
    this.identifier = value;
  }

  setPassword(value:string){
    this.password = value;
  }

  login(){
    console.log('Logging in with:',{
      username:this.identifier,
      password:this.password
    })
  }
  
}
