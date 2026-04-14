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
  @Output() deleteTour = new EventEmitter<TourItemInterface>();
  @Output() viewLogs = new EventEmitter<TourItemInterface>();

  onEditClick(): void {
    if (this.tour) {
      this.editTour.emit(this.tour);
    }
  }

  onDeleteClick(): void {
    console.log('Delete clicked in detail');
    if (this.tour) {
      this.deleteTour.emit(this.tour);
    }
  }
  
  onShowMoreClick(): void {
  if (this.tour) {
    this.viewLogs.emit(this.tour);
  }
}
}
