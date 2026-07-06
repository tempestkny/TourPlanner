import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { StatisticsDashboard } from '../../Statistics/statistics-dashboard/statistics-dashboard';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import { TourCreation } from '../tour-creation/tour-creation';
import { TourDetail } from '../tour-detail/tour-detail';
import { TourEdit } from '../tour-edit/tour-edit';
import { CreateTourLog } from '../tour-log/create-tour-log/create-tour-log';
import { EditTourLog } from '../tour-log/edit-tour-log/edit-tour-log';
import { LogListService } from '../tour-log/tour-log-list/log-list-service';
import { TourLogList } from '../tour-log/tour-log-list/tour-log-list';
import { TourLogInterface } from '../tour-log/tour-log-interface';
import { ViewTourLog } from '../tour-log/view-tour-log/view-tour-log';
import { TourList } from '../tour-list/tour-list';
import { TourListService } from '../tour-list/tour-list-service';

@Component({
  selector: 'app-tour-shell',
  imports: [
    RouterModule,
    TourList,
    TourDetail,
    TourEdit,
    TourCreation,
    TourLogList,
    CreateTourLog,
    ViewTourLog,
    EditTourLog,
    StatisticsDashboard
  ],
  templateUrl: './tour-shell.html',
  styleUrl: './tour-shell.css',
})
export class TourShell {
  constructor(
    private tourListService: TourListService,
    private logListService: LogListService
  ) {}

  mode: 'detail' | 'edit' | 'create' | 'logList' | 'logCreate' | 'logView' | 'logEdit' | 'none' = 'none';

  selectedTour: TourItemInterface | null = null;
  selectedLog: TourLogInterface | null = null;
  originalTour: TourItemInterface | null = null;

  onEditTour(tour: TourItemInterface): void {
    this.originalTour = tour;
    this.selectedTour = { ...tour };
    this.mode = 'edit';
    console.log('Selected for Edit:', tour.title);
  }

  onEditSaved(updatedTour: TourItemInterface): void {
    if (this.originalTour) {
      this.tourListService.updateTour(this.originalTour.id, updatedTour);
      this.selectedTour = updatedTour;
      this.originalTour = updatedTour;
      this.mode = 'detail';
    }
  }

  onEditCancelled(): void {
    if (this.originalTour) {
      this.selectedTour = this.originalTour;
    }
    this.mode = 'detail';
  }

  onDeleteTour(tour: TourItemInterface): void {
    this.tourListService.deleteTour(tour.id);

    this.logListService.setLogs(
      this.logListService.logs.filter(log => log.tourId !== tour.id)
    );

    this.selectedTour = null;
    this.originalTour = null;
    this.mode = 'none';
    console.log('Deleted tour:', tour.title);
  }

  onCreateTour(): void {
    this.selectedTour = null;
    this.mode = 'create';
    console.log('Create new Tour');
  }

  onCreateSuccess(tour: TourItemInterface): void {
    this.selectedTour = tour;
    this.mode = 'detail';
  }

  onCreateCancelled(): void {
    this.mode = 'none';
  }

  onSelectTour(tour: TourItemInterface): void {
    this.selectedTour = tour;
    this.logListService.loadLogs(tour.id);
    this.mode = 'detail';
    console.log('Selected for View: ', tour);
  }

  onViewLogList(tour: TourItemInterface): void {
    this.selectedTour = tour;
    this.logListService.loadLogs(tour.id);
    this.mode = 'logList';
  }

  onCreateLog(tour: TourItemInterface): void {
    this.selectedTour = tour;
    this.logListService.loadLogs(tour.id);
    this.mode = 'logCreate';
  }

  onViewLog(log: TourLogInterface): void {
    this.selectedLog = log;
    this.mode = 'logView';
  }

  onEditLog(log: TourLogInterface): void {
    this.selectedLog = log;
    this.mode = 'logEdit';
  }

  onSaveLog(updated: TourLogInterface): void {
    this.logListService.updateLog(updated);
    this.selectedLog = updated;
    this.mode = 'logView';
  }

  onDeleteLog(id: string): void {
    this.logListService.deleteLog(id);
    this.selectedLog = null;
    this.mode = 'logList';
  }

  onCancelLog(): void {
    this.mode = 'logList';
  }
}
