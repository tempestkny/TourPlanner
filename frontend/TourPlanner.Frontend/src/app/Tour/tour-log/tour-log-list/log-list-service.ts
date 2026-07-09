import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Observable, tap } from 'rxjs';
import { TourLogInterface } from '../tour-log-interface';
import { TourLogService } from '../tour-log-service';

@Injectable({
  providedIn: 'root',
})
export class LogListService {
  
  private logsSubject = new BehaviorSubject<TourLogInterface[]>([]); // service owns tourlog state, components subscribe to logs$ instead of manually shared state
  logs$ = this.logsSubject.asObservable();

  constructor(private readonly tourLogService: TourLogService) {}

  get logs(): TourLogInterface[]{ // getter for current logs value
    return this.logsSubject.value;
  } 

  setLogs(logs: TourLogInterface[]){
    this.logsSubject.next(logs);
  }

  loadLogs(tourId: string): void { // http call to backend, load logs, refreshes behaviour subject
    this.tourLogService.getLogsByTourId(tourId).subscribe({
      next: (logs) => { // response as logs
        const otherTourLogs = this.logsSubject.value.filter(log => log.tourId !== tourId); // keep logs of other tours
        this.logsSubject.next([...otherTourLogs, ...logs]); // initiates UI update
      },
      error: (error) => console.error('Failed to load tour logs', error)
    });
  }

  addLog(log: TourLogInterface): Observable<TourLogInterface> { 
    return this.tourLogService.createLog(log).pipe(
      tap((createdLog) => {
        const updated = [...this.logsSubject.value, createdLog];
        this.logsSubject.next(updated);
      })
    );
  }

  updateLog(updated: TourLogInterface){ 
    this.tourLogService.updateLog(updated.id, updated).subscribe({
      next: () => {
        const newList = this.logsSubject.value.map(
          log =>
            log.id === updated.id ? updated : log
        );
        this.logsSubject.next(newList);
      },
      error: (error) => console.error('Failed to update tour log', error)
    });
  }

  deleteLog(id:string){
    this.tourLogService.deleteLog(id).subscribe({
      next: () => {
        const updated = this.logsSubject.value.filter(log => log.id !== id);
        this.logsSubject.next(updated);
      },
      error: (error) => console.error('Failed to delete tour log', error)
    });
  }

}
