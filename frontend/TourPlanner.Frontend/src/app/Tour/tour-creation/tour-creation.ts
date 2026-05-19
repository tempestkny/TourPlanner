import { Component, EventEmitter, Output } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourListService } from '../tour-list/tour-list-service';
import * as L from 'leaflet';

@Component({
  selector: 'app-tour-creation',
  imports: [],
  templateUrl: './tour-creation.html',
  styleUrl: './tour-creation.css',
})
export class TourCreation {
  @Output() cancel = new EventEmitter<void>();
  @Output() success = new EventEmitter<TourItemInterface>()

  validationMessage = '';
  private map?: L.Map;

  newTour: TourItemInterface = {
    id: '',
    userId: '',
    title: '',
    from: '',
    to: '',
    transportType: ''
  };

  isFromValid = false;
  isToValid = false;

  setTransportType(value: string) {
    this.newTour.transportType = value;
  }

  // The boolean is set, so the error message can trigger,
  // but the string is still added to the Tour
  setTo(value: string) {
    if (this.CheckIfRealPlace(value))
      this.isToValid = true;
    else
      this.isToValid = false;
    this.newTour.to = value;
  }

  setFrom(value: string) {
    if (this.CheckIfRealPlace(value))
      this.isFromValid = true;
    else
      this.isFromValid = false;
    this.newTour.from = value;
  }

  setDescription(value: string) {
    this.newTour.tourDescription = value;
  }

  setTitle(value: string) {
    this.newTour.title = value;
  }

  constructor(private tourService: TourListService) { }

  ngOnInit(): void {
    setTimeout(() => this.initMap(), 0);
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

    if (!this.newTour.transportType.trim()) {
      this.validationMessage = 'Please select a transport type.';
      return;
    }

    this.SetTourID();
    this.tourService.addTour(this.newTour);
    this.success.emit(this.newTour);
  }

  Cancel(){
    this.cancel.emit();
  }

  SetTourID(): void {
    this.newTour.id = crypto.randomUUID();
  }

  // Should later check if leaflet can find the location.
  CheckIfRealPlace(place: string): boolean {
    if (true)
      return true;
    return false

  }

  private initMap(): void {
    if (this.map) {
      this.map.remove();
    }

    this.map = L.map('map').setView([48.2082, 16.3738], 13);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    L.marker([48.2082, 16.3738])
      .addTo(this.map)
      .bindPopup('Tour preview');
  }
}
