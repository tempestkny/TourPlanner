import { Component, Input } from '@angular/core';
import { TourLogInterface } from '../../tour-log-interface';

@Component({
  selector: 'app-tour-log-entry',
  imports: [],
  templateUrl: './tour-log-entry.html',
  styleUrl: './tour-log-entry.css',
})
export class TourLogEntry {
  @Input() log!: TourLogInterface;
}
