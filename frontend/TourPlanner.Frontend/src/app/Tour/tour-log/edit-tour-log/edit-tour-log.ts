import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Difficulty, TourLogInterface } from '../tour-log-interface';

@Component({
  selector: 'app-edit-tour-log',
  imports: [],
  templateUrl: './edit-tour-log.html',
  styleUrl: './edit-tour-log.css',
})
export class EditTourLog {

  @Input() log!: TourLogInterface | null;
  @Output() back = new EventEmitter<void>();
  @Output() save = new EventEmitter<TourLogInterface>();

  validationMessage = '';

  difficulties = Object.values(Difficulty);

  newLog!: TourLogInterface;
  ngOnChanges(changes: SimpleChanges){
    if(changes['log'] && this.log){
      this.newLog = {...this.log};
    }
  }

  formatDate(date: Date | string): string { 
    return new Date(date).toLocaleString('en-GB', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  setComment(arg0: string) {
    this.newLog.comment = arg0;
  }

  setDifficulty($event: Event) {
    const value = ($event.target as HTMLSelectElement).value;
    this.newLog.difficulty = value as Difficulty;
  }

  setDistance(arg0: number) {
    this.newLog.totalDistance = arg0;
  }

  setTime(arg0: number) {
    this.newLog.totalTime = arg0;
  }

  setRating($event: Event) {
    const value = ($event.target as HTMLSelectElement).value;
    this.newLog.rating = Number(value);
  }

  CheckNumber(number?: number): boolean {
    return number != null && !Number.isNaN(number) && number > 0;
  }

  onSave() {
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
      
    const updated: TourLogInterface = {
      ...this.newLog,
      id: this.log!.id
    }

    this.save.emit(updated);
    this.back.emit();
  }
  
  onCancel() {
    this.back.emit();
  }
}
