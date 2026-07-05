import { Component, EventEmitter, Output } from '@angular/core';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import { TourListService } from '../tour-list/tour-list-service';
import { TransportType } from '../interfaces/tour-interface/transport-type';
import { Coordinates } from '../interfaces/tour-interface/coordinates';
import { OpenRouteService } from '../open-route-service';
import { TourMapComponent } from "../tour-map-component/tour-map-component";

@Component({
  selector: 'app-tour-creation',
  imports: [TourMapComponent],
  templateUrl: './tour-creation.html',
  styleUrl: './tour-creation.css',
})
export class TourCreation {
  @Output() cancel = new EventEmitter<void>();
  @Output() success = new EventEmitter<TourItemInterface>()

  validationMessage = '';

  newTour: TourItemInterface = {
    id: '',
    title: '',
    description: '',
    from: '',
    to: '',
    transportType: null,
    route: null
  };

  isFromValid = false;
  isToValid = false;
  isTransportTypeSet = false;

  fromCoord: Coordinates | undefined;
  toCoord: Coordinates | undefined;

  constructor(private tourListService: TourListService, private openRouteService: OpenRouteService) { }


  // The boolean is set, so the error message can trigger,
  // but the string is still added to the Tour
  setTo(value: string) {
    this.isToValid = false;
    this.newTour.to = value;
  }

  setFrom(value: string) {
    this.isFromValid = false;
    this.newTour.from = value;
  }

  setTransportType(value: string) {
    this.newTour.transportType = this.transportMap[value] ?? TransportType.Car;
    if(this.isFromValid && this.isToValid){
      this.generateRoute();
    }
  }


  async setPoints() {
    if (!this.isFromValid)
      this.openRouteService.GetCoordinatesOfPlace(this.newTour.from).subscribe({
        next: coord => {
          this.isFromValid = true;
          this.fromCoord = coord;
          this.generateRoute();
        },
        error: () => {
          this.isFromValid = false;
          this.fromCoord = undefined;
        }
      });

    if (!this.isToValid)
      this.openRouteService.GetCoordinatesOfPlace(this.newTour.to).subscribe({
        next: coord => {
          this.isToValid = true;
          this.toCoord = coord;
          this.generateRoute();
        },
        error: () => {
          this.isToValid = false;
          this.toCoord = undefined;
        }
      });
  }

  setDescription(value: string) {
    this.newTour.description = value;
  }

  setTitle(value: string) {
    this.newTour.title = value;
  }


  generateRoute() {
    if (this.isFromValid && this.isToValid && this.newTour.transportType) {
      this.openRouteService.GetRouteInformation({ start: this.fromCoord!, dest: this.toCoord!, profile: this.newTour.transportType }).subscribe({
        next: (route) => {
          this.newTour.route = route
        },
        error: () => {
          this.newTour.route = null;
          console.error("Route could not be generated");
        }
      });
    }
  }

  // The tour can only be created when all neccesary fields were filled out.
  CreateTour() {
    this.validationMessage = '';

    if (!this.newTour.title.trim()) {
      this.validationMessage = 'Please enter a tour title.';
      return;
    }

    if (!this.newTour.from.trim()) {
      this.validationMessage = 'Please enter a start location.';
      return;
    }

    if (!this.newTour.to.trim()) {
      this.validationMessage = 'Please enter a destination.';
      return;
    }

    if (!this.isFromValid || !this.isToValid) {
      this.validationMessage = 'Please enter valid locations.';
      return;
    }

    if (!this.newTour.transportType) {
      this.validationMessage = 'Please select a transport type.';
      return;
    }

    if (!this.newTour.route) {
      this.validationMessage = 'Please generate the route.';
      return;
    }

    this.tourListService.addTour(this.newTour);
    this.success.emit(this.newTour);
  }

  Cancel() {
    this.cancel.emit();
  }


  private transportMap: Record<string, TransportType> = {
    Car: TransportType.Car,
    Bike: TransportType.Bike,
    Hike: TransportType.Hike
  }
}
