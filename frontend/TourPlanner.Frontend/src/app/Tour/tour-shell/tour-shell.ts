import { Component } from '@angular/core';
import { RouterModule } from "@angular/router";
import { TourList } from "../tour-list/tour-list";
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourDetail } from "../tour-detail/tour-detail";
import { TourEdit } from "../tour-edit/tour-edit";
import { TourCreation } from "../tour-creation/tour-creation";
import { TourLogList } from '../tour-log/tour-log-list/tour-log-list';
import { CreateTourLog } from "../tour-log/create-tour-log/create-tour-log";
import { TourLogInterface } from '../tour-log/tour-log-interface';
import { ViewTourLog } from "../tour-log/view-tour-log/view-tour-log";


@Component({
  selector: 'app-tour-shell',
  imports: [RouterModule, TourList, TourDetail, TourEdit, TourCreation, TourLogList, CreateTourLog, ViewTourLog],
  templateUrl: './tour-shell.html',
  styleUrl: './tour-shell.css',
})

export class TourShell {

  mode: 'detail' | 'edit' | 'create' | 'logList' | 'logCreate' | 'logView' | 'none' = 'detail';

  selectedTour: TourItemInterface | null = null;
  selectedLog: TourLogInterface | null = null;

  // When the button to edit/create/view Tour is clicked
  // the TourItemInterface of this object is selected 
  // for CRUD-Operations

  onEditTour(tour: TourItemInterface) {
    this.selectedTour = tour;
    this.mode = 'edit';
    console.log("Selected for Edit: ", tour.title);
  }

  onCreateTour() {
    this.selectedTour = null;
    this.mode = 'create';
    console.log("Create new Tour");
  }

  onSelectTour(tour: TourItemInterface) {
    this.selectedTour = tour;
    this.mode = 'detail';
    console.log("Selected for View: ", tour.title);
  }

  // Log User-Interface

  onViewLogList(tour: TourItemInterface) {
    this.selectedTour = tour;
    this.mode = 'logList';
  }

  onCreateLog(tour: TourItemInterface) {
    this.selectedTour = tour;
    this.mode = 'logCreate';
    console.log("Create new Tour Log");
  }

  onViewLog(log: TourLogInterface) {
    this.selectedLog = log;
    this.mode = 'logView';
    console.log("View Tour Log", log);
  }


  onCancelLog() {
    this.mode = 'logList';
  }

}
