import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TourLogInterface } from '../tour-log-interface';

@Component({
  selector: 'app-view-tour-log',
  imports: [],
  templateUrl: './view-tour-log.html',
  styleUrl: './view-tour-log.css',
})
export class ViewTourLog {

  @Input() log!: TourLogInterface | null
  @Output() back = new EventEmitter<void>();

  onBack() {
    this.back.emit();
  }
}
