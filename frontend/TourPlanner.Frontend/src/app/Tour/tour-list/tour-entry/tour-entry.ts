import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TourItemInterface } from '../../tour-item/tour-item-interface';

@Component({
  selector: 'app-tour-entry',
  imports: [],
  templateUrl: './tour-entry.html',
  styleUrl: './tour-entry.css',
})
export class TourEntry {
  @Input() tour!: TourItemInterface;

  // Tour Logs
  @Output() viewLogs = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>();
}
