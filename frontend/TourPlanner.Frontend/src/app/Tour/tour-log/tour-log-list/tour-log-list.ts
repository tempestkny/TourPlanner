import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TourLogEntry } from "./tour-log-entry/tour-log-entry";
import { LogListService } from './log-list-service';
import { AsyncPipe } from '@angular/common';
import { TourLogInterface } from '../tour-log-interface';
import { Observable } from 'rxjs';
import { map } from 'rxjs';
import { TourItemInterface } from '../../interfaces/tour-interface/tour-item-interface';

@Component({
  selector: 'app-tour-log-list',
  imports: [TourLogEntry, AsyncPipe],
  templateUrl: './tour-log-list.html',
  styleUrl: './tour-log-list.css',
})
export class TourLogList {
  @Input() tour!: TourItemInterface | null;
  @Output() viewLog = new EventEmitter<TourLogInterface>();

  logs$: Observable<TourLogInterface[]> = new Observable<TourLogInterface[]>();

  constructor(private logListService: LogListService) {}

  ngOnChanges(): void {
    if (this.tour) {
      this.logListService.loadLogs(this.tour.id);
      this.logs$ = this.logListService.logs$.pipe(
        map(logs => logs.filter(log => log.tourId === this.tour?.id))
      );
    }
  }

  onViewLog(log: TourLogInterface) {
    this.viewLog.emit(log);
    console.log("TourLogList: ", log);
  }
}
