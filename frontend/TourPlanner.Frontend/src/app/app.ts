import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TourList } from "./Tour/tour-list/tour-list";

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('TourPlanner.Frontend');
}
