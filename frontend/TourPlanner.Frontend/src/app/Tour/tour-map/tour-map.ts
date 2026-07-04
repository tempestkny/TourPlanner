import { Component } from '@angular/core';
import L from 'leaflet';

@Component({
  selector: 'app-tour-map',
  imports: [],
  templateUrl: './tour-map.html',
  styleUrl: './tour-map.css',
})
export class TourMap {

    private map?: L.Map;

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
