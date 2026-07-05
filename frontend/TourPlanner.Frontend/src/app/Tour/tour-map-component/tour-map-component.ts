import { Component, Input } from '@angular/core';
import { TourItemInterface } from '../interfaces/tour-interface/tour-item-interface';
import L from 'leaflet';

const defaultIcon = L.icon({
  iconUrl: 'assets/img/marker-icon.png',
  shadowUrl: 'assets/img/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
});

L.Marker.prototype.options.icon = defaultIcon;

@Component({
  selector: 'app-tour-map-component',
  imports: [],
  templateUrl: './tour-map-component.html',
  styleUrl: './tour-map-component.css',
})

export class TourMapComponent {
  @Input() tour!: TourItemInterface;

  private map?: L.Map;

  ngAfterViewInit() {
    this.initializeMap();
  }

  ngOnChanges() {
    if (this.map) {
      this.map.remove();   // alte Map komplett entfernen
      this.map = undefined;
    }

    this.initializeMap();  // neue Map erzeugen
  }

  private initializeMap(): void {
    if (this.map) {
      this.map.remove();
    }

    this.map = L.map('map');

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors'
    }).addTo(this.map);

    console.log("Displayed Tour: ",JSON.stringify(this.tour))
    setTimeout(() => this.drawRoute(), 0);
  }

  private drawRoute(): void {
    if (!this.map || !this.tour?.route?.route){
      console.log("Tour has no route.");
      return;} 

    const coords = this.tour.route.route;
    

    // ORS [lon, lat] → Leaflet [lat, lon]
    const latLngs: L.LatLngExpression[] = coords.map(c => [c.lat, c.lon]);

    // Polyline zeichnen
    const polyline = L.polyline(latLngs, {
      color: 'blue',
      weight: 4
    }).addTo(this.map);

    // Karte auf Route zoomen
    this.map.fitBounds(polyline.getBounds());

    L.marker(latLngs[0]).addTo(this.map).bindPopup("Start");
    L.marker(latLngs[latLngs.length - 1]).addTo(this.map).bindPopup("Ziel");

  }
}
