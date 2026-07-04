import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TourLogInterface } from '../tour-log-interface';
import { TourLogService } from '../tour-log-service';

@Injectable({
  providedIn: 'root',
})
export class LogListService {
  
  private logsSubject = new BehaviorSubject<TourLogInterface[]>([]);
  logs$ = this.logsSubject.asObservable();

  constructor(private readonly tourLogService: TourLogService) {}

  get logs(): TourLogInterface[]{
    return this.logsSubject.value;
  } 

  setLogs(logs: TourLogInterface[]){
    this.logsSubject.next(logs);
  }

  loadLogs(tourId: string): void {
    this.tourLogService.getLogsByTourId(tourId).subscribe({
      next: (logs) => {
        const otherTourLogs = this.logsSubject.value.filter(log => log.tourId !== tourId);
        this.logsSubject.next([...otherTourLogs, ...logs]);
      },
      error: (error) => console.error('Failed to load tour logs', error)
    });
  }

  addLog(log: TourLogInterface){
    this.tourLogService.createLog(log).subscribe({
      next: (createdLog) => {
        const updated = [...this.logsSubject.value, createdLog];
        this.logsSubject.next(updated);
      },
      error: (error) => console.error('Failed to create tour log', error)
    });
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
