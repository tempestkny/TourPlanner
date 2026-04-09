import { Routes } from '@angular/router';
import { UserLogin } from './User/user-login/user-login';
import { UserRegister } from './User/user-register/user-register';
import { TourList } from './Tour/tour-list/tour-list';

export const routes: Routes = [
    {path:'',component : UserLogin},
    {path:'login',component : UserLogin},
    {path:'register',component: UserRegister},
    {path:'tour-list',component:TourList}
];
