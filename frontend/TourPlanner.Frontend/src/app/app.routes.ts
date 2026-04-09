import { Routes } from '@angular/router';
import { TourList } from './Tour/tour-list/tour-list';
import { TourCreation } from './Tour/tour-creation/tour-creation';
import { TourShell } from './Tour/tour-shell/tour-shell';
import { TourDetail } from './Tour/tour-detail/tour-detail';
import { TourEdit } from './Tour/tour-edit/tour-edit';
import { UserLogin } from './User/user-login/user-login';
import { UserRegister } from './User/user-register/user-register';

export const routes: Routes = [
    {
        path: 'tours',
        component: TourShell,
        children: [
            { path: '',redirectTo:'tour-list', pathMatch:'full' }, // left side
            { path: 'tour-list', component: TourList }, // left side
            { path: 'details/:id', component: TourDetail }, // right side
            { path: 'edit/:id', component: TourEdit }, // right side
            { path: 'create', component: TourCreation } // right side
        ]
    },
    { path: '',redirectTo:'login',pathMatch:'full'},
    { path: 'login',component:UserLogin},
    { path: 'register',component:UserRegister},
   
];
