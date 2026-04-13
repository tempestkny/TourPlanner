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
    if (number == null || Number.isNaN(number)) return false;
    return true;
  }

  constructor(private logListService: LogListService){}

  createLog() {
    if (this.CheckNumber(this.newLog.totalDistance) &&
      this.CheckNumber(this.newLog.totalTime)) {
      
        const log: TourLogInterface = {
          ...this.newLog,
          id: crypto.randomUUID(),
        }

        this.logListService.addLog(log);

        console.log("Creating: ", log);
        
        this.cancel.emit();
    }
    else {
      console.log("Could not create Tourlog: invalid input");
    }
  }


  onCancel() {
    this.cancel.emit();
  }

}
