import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';

import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import { TransportType } from '../interfaces/tour-interface/transport-type';
import { Coordinates } from '../interfaces/tour-interface/coordinates';
import { OpenRouteService } from '../open-route-service';
import { TourMapComponent } from "../tour-map-component/tour-map-component";

@Component({
  selector: 'app-tour-edit',
  standalone: true,
  imports: [TourMapComponent],
  templateUrl: './tour-edit.html',
  styleUrl: './tour-edit.css',
})
export class TourEdit implements OnChanges {
  @Input() tour!: TourItemInterface | null;

  @Output() saved = new EventEmitter<TourItemInterface>();
  @Output() cancel = new EventEmitter<void>();
  @ViewChild(TourMapComponent) mapComp!: TourMapComponent;
  validationMessage = '';
  errorMsg = '';

  editableTour: TourItemInterface = {
    id: '',
    title: '',
    description: '',
    from: '',
    to: '',
    transportType: null,
    route: null
  };

  isFromValid = true;
  isToValid = true;

  start: Coordinates | undefined;
  dest: Coordinates | undefined;

  constructor(
    private openRouteService: OpenRouteService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tour'] && this.tour) {
      this.editableTour = { ...this.tour };
      this.start = this.tour.route?.route.at(0);
      this.dest = this.tour.route?.route.at(-1);
      this.isFromValid = true;
      this.isToValid = true;
    }
  }

  setTitle(value: string): void {
    this.editableTour.title = value;
  }

  setDescription(value: string): void {
    this.editableTour.description = value;
  }

  setFrom(value: string): void {
    this.isFromValid = false;
    this.editableTour.from = value;
  }

  setTo(value: string): void {
    this.isToValid = false;
    this.editableTour.to = value;
  }

  async setPoints() {
    if (!this.isFromValid)
      this.openRouteService.GetCoordinatesOfPlace(this.editableTour.from).subscribe({
        next: coord => {
          this.isFromValid = true;
          this.start = coord;
          this.generateRoute();
        },
        error: () => {
          this.isFromValid = false;
          this.start = undefined;
          this.errorMsg = 'Please set valid locations';
          this.cdr.detectChanges();
        }
      });
    if (!this.isToValid)
      this.openRouteService.GetCoordinatesOfPlace(this.editableTour.to).subscribe({
        next: coord => {
          this.isToValid = true;
          this.dest = coord;
          this.generateRoute();
        },
        error: () => {
          this.isToValid = false;
          this.dest = undefined;
          this.errorMsg = 'Please set valid locations';
          this.cdr.detectChanges();
        }
      });
  }

  setTransportType(value: string): void {
    this.editableTour.transportType = this.transportMap[value] ?? TransportType.Car;
    if (this.isFromValid && this.isToValid) {
      this.generateRoute();
    }
  }

  generateRoute() {
    if (this.isFromValid && this.isToValid && this.editableTour.transportType) {
      this.openRouteService.GetRouteInformation({ start: this.start!, dest: this.dest!, profile: this.editableTour.transportType }).subscribe({
        next: (route) => {
          this.editableTour.route = route
          this.cdr.detectChanges();
          this.mapComp.ngOnInit();
        },
        error: () => {
          this.errorMsg = 'The Route could not be generated.';
          this.editableTour.route = null;
          console.error("Route could not be generated");
          this.cdr.detectChanges();
        }
      });
    }
  }

  saveTour(): void {
    this.validationMessage = '';

    if (!this.editableTour.title.trim()) {
      this.validationMessage = 'Please enter a tour title.';
      return;
    }

    if (!this.editableTour.from.trim()) {
      this.validationMessage = 'Please enter a start location.';
      return;
    }

    if (!this.editableTour.to.trim()) {
      this.validationMessage = 'Please enter a destination.';
      return;
    }

    if (!this.isFromValid || !this.isToValid) {
      this.validationMessage = 'Please enter valid locations.';
      return;
    }

    if (!this.editableTour.route) {
      this.validationMessage = 'Please generate the route.';
      return;
    }

    if (!this.editableTour.transportType) {
      this.validationMessage = 'Please select a transport type.';
      return;
    }

    this.saved.emit(this.editableTour);
  }

  private transportMap: Record<string, TransportType> = {
    Car: TransportType.Car,
    Bike: TransportType.Bike,
    Hike: TransportType.Hike
  }
}
