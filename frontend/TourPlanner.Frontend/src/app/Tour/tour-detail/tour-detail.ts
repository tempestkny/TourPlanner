import { Component, Input } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';

@Component({
  selector: 'app-tour-detail',
  imports: [],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.css',
})
export class TourDetail {
  @Input() tour!:TourItemInterface | null
}
