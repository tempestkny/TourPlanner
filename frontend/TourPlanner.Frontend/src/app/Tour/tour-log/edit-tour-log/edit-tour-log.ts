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

  difficulties = Object.values(Difficulty);

  newLog!: TourLogInterface;
  ngOnChanges(changes: SimpleChanges){
    if(changes['log'] && this.log){
      this.newLog = {...this.log};
    }
  }

  setDateTime(arg0: string) {
    this.newLog.timeStamp = new Date(arg0);
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

  CheckNumber(arg0: any): boolean {
    if (arg0 == null || Number.isNaN(arg0)) return false;
    return true;
  }

  onSave() {
    if (this.CheckNumber(this.newLog.totalDistance) &&
      this.CheckNumber(this.newLog.totalTime)) {
      
        const updated: TourLogInterface = {
        ...this.newLog,
        id: this.log!.id
      };

      this.save.emit(updated);
      this.back.emit();
    }
    else {
      console.log("Could not create Tourlog: invalid input");
    }
  }


  onCancel() {
    this.back.emit();
  }





}
