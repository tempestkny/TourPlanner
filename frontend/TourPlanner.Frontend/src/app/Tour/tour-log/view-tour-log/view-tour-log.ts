import { Component, Input } from '@angular/core';
import { TourLogInterface } from '../tour-log-interface';

@Component({
  selector: 'app-view-tour-log',
  imports: [],
  templateUrl: './view-tour-log.html',
  styleUrl: './view-tour-log.css',
})
export class ViewTourLog {
  @Input() log!: TourLogInterface | null
}
