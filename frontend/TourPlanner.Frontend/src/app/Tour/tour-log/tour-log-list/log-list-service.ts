import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TourLogInterface } from '../tour-log-interface';

@Injectable({
  providedIn: 'root',
})
export class LogListService {
  
  private logsSubject = new BehaviorSubject<TourLogInterface[]>([]);
  logs$ = this.logsSubject.asObservable();

  get logs(): TourLogInterface[]{
    return this.logsSubject.value;
  } 

  setLogs(logs: TourLogInterface[]){
    this.logsSubject.next(logs);
  }

  addLog(log: TourLogInterface){
    const updated = [...this.logsSubject.value, log];
    this.logsSubject.next(updated);
  }

}
