import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';

@Component({
  selector: 'app-tour-detail',
  imports: [],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.css',
})
export class TourDetail {
  @Input() tour!:TourItemInterface | null
  @Output() editTour = new EventEmitter<TourItemInterface>();

  onEditClick() {
    if (this.tour) {
      this.editTour.emit(this.tour);
    }
  } 
}
