import { Component } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourListService } from '../tour-list/tour-list-service';

@Component({
  selector: 'app-tour-creation',
  imports: [],
  templateUrl: './tour-creation.html',
  styleUrl: './tour-creation.css',
})
export class TourCreation {

  newTour: TourItemInterface = {
    id: '',
    userId: '',
    title: '',
    from: '',
    to: '',
  };

  isFromValid = false;
  isToValid = false;

  setTransportType(arg0: string) {
    this.newTour.transportType = arg0;
  }

  // The boolean is set, so the error message can trigger,
  // but the string is still added to the Tour
  setTo(arg0: string) {
    if (this.CheckIfRealPlace(arg0))
      this.isToValid = true;
    else
      this.isToValid = false;
    this.newTour.to = arg0;
  }

  setFrom(arg0: string) {
    if (this.CheckIfRealPlace(arg0))
      this.isFromValid = true;
    else
      this.isFromValid = false;
    this.newTour.from = arg0;
  }
  setDescription(arg0: string) {
    this.newTour.tourDescription = arg0;
  }
  setTitle(arg0: string) {
    this.newTour.title = arg0;
  }

  constructor(private tourService: TourListService){}
  
  // The tour can only be created when all neccesary fields were filled out.
  CreateTour() {
    if(this.isFromValid && this.isToValid){
      this.tourService.addTour(this.newTour);
      console.log("Successfully created tour", this.newTour)
    }
  }

  SetTourID() : void{
    this.newTour.id = crypto.randomUUID();
  }

  // Should later check if leaflet can find the location.
  CheckIfRealPlace(place: string): boolean {
    if (place === "Place")
      return true;
    return false

  }
}
