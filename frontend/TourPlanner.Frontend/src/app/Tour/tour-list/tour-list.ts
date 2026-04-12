import { Component, EventEmitter, Output, Signal } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourEntry } from "./tour-entry/tour-entry";
import { RouterModule } from "@angular/router";
import { TourListService } from './tour-list-service';

@Component({
  selector: 'app-tour-list',
  imports: [TourEntry, RouterModule],
  templateUrl: './tour-list.html',
  styleUrl: './tour-list.css',
})
export class TourList {
  @Output() selectTour = new EventEmitter<TourItemInterface>();
  @Output() editTour = new EventEmitter<TourItemInterface>();
  @Output() createTour = new EventEmitter<void>();

  // Tour Log events
  @Output() viewLogList = new EventEmitter<TourItemInterface>();
  @Output() createLog = new EventEmitter<TourItemInterface>()

  tours: TourItemInterface[] = [];

  constructor(private tourListService: TourListService) { }

  ngOnInit() {
    this.tourListService.tours$.subscribe(tours => {
      this.tours = tours;
    });
  }

  onQueryInput(arg0: string) {

  }


  clearQuery() {

  }

}
