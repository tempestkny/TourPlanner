import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TourItemInterface } from '../../interfaces/tour-interface/tour-item-interface';

@Component({
  selector: 'app-tour-entry',
  imports: [],
  templateUrl: './tour-entry.html',
  styleUrl: './tour-entry.css',
})
export class TourEntry {
  @Input() tour!: TourItemInterface;

  // Tour Logs
  @Output() viewLogList = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>();
}
