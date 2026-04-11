import { Component, Input } from '@angular/core';
import { TourItemInterface } from '../../tour-item/tour-item-interface';

@Component({
  selector: 'app-tour-log-list',
  imports: [],
  templateUrl: './tour-log-list.html',
  styleUrl: './tour-log-list.css',
})
export class TourLogList {
  @Input() tour!: TourItemInterface | null;
}
