import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Difficulty, TourLogInterface } from '../tour-log-interface';
import { LogListService } from '../tour-log-list/log-list-service';
import { TourItemInterface } from '../../interfaces/tour-interface/tour-item-interface';

@Component({
  selector: 'app-create-tour-log',
  imports: [],
  templateUrl: './create-tour-log.html',
  styleUrl: './create-tour-log.css',
})

//a tour-log consists of date/time, comment, difficulty, total distance, total time, and rating taken
// on the tour
export class CreateTourLog {
  @Input() tour!: TourItemInterface // tour comes from tourshell
  @Output() cancel = new EventEmitter<void>(); // tells tourshell to close the create log form

  validationMessage = '';
  isSaving = false; // deactivates button, prevents multiple submissions

  newLog: TourLogInterface = {
    id: '',
    tourId: '',
    timeStamp: new Date(),
    totalDistance: 0,
    totalTime: 0,
    difficulty: Difficulty.Easy,
    rating:1
  }

  difficulties = Object.values(Difficulty); // difficulty options for dropdown

  currentDateTime = new Date().toISOString().slice(0, 16)
  // format: "1900-01-01T00:00"

  setDateTime(arg0: string) { // setter
    this.newLog.timeStamp = new Date(arg0);
  }

  setComment(arg0: string) {
    this.newLog.comment = arg0;
  }

  setDifficulty(arg0: Event) {
    const value = (arg0.target as HTMLSelectElement).value;
    this.newLog.difficulty = value as Difficulty;
  }

  setDistance(arg0: number) {
    this.newLog.totalDistance = arg0;
  }

  setTime(arg0: number) {
    this.newLog.totalTime = arg0;
  }

  setRating(arg0: Event) {
    const value = (arg0.target as HTMLSelectElement).value;
    this.newLog.rating = Number(value);
  }

  CheckNumber(number?: number): boolean { // validation for total distance and total time, must be greater than 0
    return number != null && !Number.isNaN(number) && number > 0;
  }

  constructor(private logListService: LogListService){}

  createLog() {
    this.validationMessage = '';

    if (!this.tour?.id) {
      this.validationMessage = 'Please select a tour before creating a log.';
      return;
    }

    if (!this.newLog.timeStamp || Number.isNaN(this.newLog.timeStamp.getTime())) {
      this.validationMessage = 'Please select a valid date and time.';
      return;
    }

    if (!this.newLog.difficulty) {
      this.validationMessage = 'Please select a difficulty.';
      return;
    }

    if (!this.CheckNumber(this.newLog.totalDistance)) {
      this.validationMessage = 'Please enter a total distance greater than 0.';
      return;
    }

    if (!this.CheckNumber(this.newLog.totalTime)) {
      this.validationMessage = 'Please enter a total time greater than 0.';
      return;
    }

    if (!this.newLog.rating || this.newLog.rating < 1 || this.newLog.rating > 5) {
      this.validationMessage = 'Please select a rating between 1 and 5.';
      return;
    }

    const log: TourLogInterface = { // log is created, with empty id and tourId, which will be set by the backend
      ...this.newLog,
      id: '',
      tourId: this.tour.id
    };

    this.isSaving = true;
    this.logListService.addLog(log).subscribe({
      next: () => {
        this.isSaving = false;
        this.cancel.emit();
      },
      error: (error) => {
        console.error('Failed to create tour log', error);
        this.validationMessage = this.getErrorMessage(error);
        this.isSaving = false;
      }
    });
}


  onCancel() {
    this.cancel.emit();
  }

  private getErrorMessage(error: unknown): string { // backend validation error handling
    if (error instanceof HttpErrorResponse && error.status === 400) {
      const validationErrors = error.error?.errors;
      if (validationErrors) {
        return Object.values(validationErrors).flat().join(' ');
      }

      return error.error?.detail ?? 'The tour log data is invalid.';
    }

    return 'Could not create tour log.';
  }
}
