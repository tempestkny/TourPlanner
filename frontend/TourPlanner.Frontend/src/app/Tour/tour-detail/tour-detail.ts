import { ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, Output, SimpleChanges } from '@angular/core';
import { TourLogInterface } from '../tour-log/tour-log-interface';
import { LogListService } from '../tour-log/tour-log-list/log-list-service';
import { TourLogEntry } from '../tour-log/tour-log-list/tour-log-entry/tour-log-entry';
import * as L from 'leaflet';
import { Subscription } from 'rxjs';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import { RouteInformation } from '../interfaces/tour-dtos/route-information';
import { TourMapComponent } from "../tour-map-component/tour-map-component";

@Component({
  selector: 'app-tour-detail',
  imports: [TourLogEntry, TourMapComponent],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.css',
})
export class TourDetail {
  @Input() tour!: TourItemInterface | null
  @Output() editTour = new EventEmitter<TourItemInterface>();
  @Output() deleteTour = new EventEmitter<TourItemInterface>();

  @Output() viewLogs = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>();
  @Output() viewLog = new EventEmitter<TourLogInterface>();

  tourLogs: TourLogInterface[] = [];
  private logsSubscription?: Subscription;

  constructor(
    private logListService: LogListService,
    private changeDetector: ChangeDetectorRef
  ) { }

  getAverageRating(): number {
    return this.calculateAverageRating(this.tourLogs.map(log => log.rating!));
  }

  calculateAverageRating(ratings: number[]): number {
    if (ratings.length == 0) return -1;
    const sum = ratings.reduce((acc, cur) => acc + cur, 0);
    return sum / ratings.length
  }

  getStars(avg: number): string {
    const stars = Math.round(avg);
    if (avg < 0) return '';
    return '★'.repeat(stars) + '☆'.repeat(5 - stars)
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.tour) {
      this.logListService.loadLogs(this.tour.id);
      this.logsSubscription?.unsubscribe();
      this.logsSubscription = this.logListService.logs$.subscribe(logs => {
        this.tourLogs = logs.filter(log => log.tourId === this.tour?.id);
        this.changeDetector.detectChanges();
      });
    }
  }

  ngOnDestroy(): void {
    this.logsSubscription?.unsubscribe();
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
