import { Component } from '@angular/core';
import { RouterModule } from "@angular/router";
import { TourList } from "../tour-list/tour-list";
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourDetail } from "../tour-detail/tour-detail";
import { TourEdit } from "../tour-edit/tour-edit";
import { TourCreation } from "../tour-creation/tour-creation";


@Component({
  selector: 'app-tour-shell',
  imports: [RouterModule, TourList, TourDetail, TourEdit, TourCreation],
  templateUrl: './tour-shell.html',
  styleUrl: './tour-shell.css',
})

export class TourShell {
  mode: 'detail' | 'edit' | 'create' | 'none' = 'none';
  selectedTour: TourItemInterface | null = null;


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

}
