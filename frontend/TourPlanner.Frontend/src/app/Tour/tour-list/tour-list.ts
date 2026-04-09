import { Component, EventEmitter, Output } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourEntry } from "./tour-entry/tour-entry";
import { RouterModule } from "@angular/router";

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


  tours: TourItemInterface[] = [
    {
      name: 'Vienna City Walk',
      tourDescription: 'A relaxing walk through the historic center.',
      from: 'Stephansplatz',
      to: 'Schönbrunn',
      transportType: 'Walking'
    },
    {
      name: 'Danube Bike Tour',
      tourDescription: 'A scenic bike ride along the river.',
      from: 'Donauinsel',
      to: 'Klosterneuburg',
      transportType: 'Bike'
    }
  ];

  onQueryInput(arg0: string) {

  }


  clearQuery() {

  }

}
