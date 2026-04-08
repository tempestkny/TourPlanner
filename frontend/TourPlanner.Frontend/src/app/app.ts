import { Component, signal } from '@angular/core';
import { RouterOutlet } from "../../node_modules/@angular/router/types/_router_module-chunk";
import { UserLogin } from './User/user-login/user-login';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('TourPlanner.Frontend');
}
