import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Tour } from "./tour/tour";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Tour],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('TourPlanner.Frontend');
}
