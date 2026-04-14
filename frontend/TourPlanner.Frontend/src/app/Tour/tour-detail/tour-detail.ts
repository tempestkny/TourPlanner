import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourLogInterface } from '../tour-log/tour-log-interface';
import { LogListService } from '../tour-log/tour-log-list/log-list-service';
import { TourLogEntry } from '../tour-log/tour-log-list/tour-log-entry/tour-log-entry';

@Component({
  selector: 'app-tour-detail',
  imports: [TourLogEntry],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.css',
})
export class TourDetail {
  @Input() tour!:TourItemInterface | null
  @Output() editTour = new EventEmitter<TourItemInterface>();
  @Output() deleteTour = new EventEmitter<TourItemInterface>();

  @Output() viewLogs = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>();
  @Output() viewLog = new EventEmitter<TourLogInterface>();

  tourLogs: TourLogInterface[] = [];

  constructor(private logListService: LogListService) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.tourLogs = this.logListService.logs;
  }

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

  onAddLogClick(): void {
    if (this.tour) {
      this.createLog.emit(this.tour);
    }
  }

  onViewLogClick(log: TourLogInterface): void {
    this.viewLog.emit(log);
  }
  
  onShowMoreClick(): void {
  if (this.tour) {
    this.viewLogs.emit(this.tour);
  }
}
}
