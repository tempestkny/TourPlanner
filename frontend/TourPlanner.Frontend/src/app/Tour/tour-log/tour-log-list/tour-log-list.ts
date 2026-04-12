import { Component, Input } from '@angular/core';
import { TourItemInterface } from '../../tour-item/tour-item-interface';
import { TourLogEntry } from "./tour-log-entry/tour-log-entry";
import { LogListService } from './log-list-service';
import { AsyncPipe } from '@angular/common';
import { TourLogInterface } from '../tour-log-interface';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-tour-log-list',
  imports: [TourLogEntry, AsyncPipe],
  templateUrl: './tour-log-list.html',
  styleUrl: './tour-log-list.css',
})
export class TourLogList {
  @Input() tour!: TourItemInterface | null;

  logs$ : Observable<TourLogInterface[]>;


  constructor(private logListService: LogListService) {
      this.logs$ = this.logListService.logs$;
  } 
}
