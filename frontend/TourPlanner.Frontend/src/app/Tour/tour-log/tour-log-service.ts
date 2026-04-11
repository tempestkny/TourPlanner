import { Injectable, signal } from '@angular/core';
import { TourLogInterface } from './tour-log-interface';

@Injectable({
  providedIn: 'root',
})

export class TourLogService {
  private _logs = signal<TourLogInterface[]>([]);
  logs = this._logs.asReadonly();

  addLog(log: TourLogInterface) {
    this._logs.update(old => [...old, log]);
  }

  updateLog(id: string, partial: Partial<TourLogInterface>) {
    this._logs.update(old =>
      old.map(log =>
        log.id === id ? { ...log, ...partial } : log
      )
    );
  }

  deleteLog(id:string) {
    this._logs.update(old =>
      old.filter(log => log.id !== id)
    );
  }

  getLogsForTour(tourId: string) {
    return this.logs().filter(log => log.id === tourId);
  }
}
