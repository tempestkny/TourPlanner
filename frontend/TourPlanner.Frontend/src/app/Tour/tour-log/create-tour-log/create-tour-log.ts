import { Component, EventEmitter, Output } from '@angular/core';
import { Difficulty, TourLogInterface } from '../tour-log-interface';
import { LogListService } from '../tour-log-list/log-list-service';

@Component({
  selector: 'app-create-tour-log',
  imports: [],
  templateUrl: './create-tour-log.html',
  styleUrl: './create-tour-log.css',
})

//a tour-log consists of date/time, comment, difficulty, total distance, total time, and rating taken
// on the tour
export class CreateTourLog {
  @Output() cancel = new EventEmitter<void>();

  validationMessage = '';

  newLog: TourLogInterface = {
    id: '',
    timeStamp: new Date(),
    totalDistance: 0,
    totalTime: 0,
    difficulty: Difficulty.Easy,
    rating:1
  }

  difficulties = Object.values(Difficulty);

  currentDateTime = new Date().toISOString().slice(0, 16)
  // format: "1900-01-01T00:00"

  setDateTime(arg0: string) {
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

  CheckNumber(number?: number): boolean {
    return number != null && !Number.isNaN(number) && number > 0;
  }

  constructor(private logListService: LogListService){}

  createLog() {
    this.validationMessage = '';

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

    const log: TourLogInterface = {
      ...this.newLog,
      id: crypto.randomUUID(),
    };

    this.logListService.addLog(log);
    console.log('Creating: ', log);
    this.cancel.emit();
}


  onCancel() {
    this.cancel.emit();
  }

}
