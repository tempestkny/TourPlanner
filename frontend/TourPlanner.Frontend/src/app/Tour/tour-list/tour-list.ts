import { Component } from '@angular/core';

@Component({
  selector: 'app-tour-list',
  imports: [],
  templateUrl: './tour-list.html',
  styleUrl: './tour-list.css',
})
export class TourList {

  tours = [
    { name: 'Tour 1' },
    { name: 'Tour 2' },
    { name: 'Tour 3' }
  ];

  onQueryInput(arg0: string) {
    
  }


  clearQuery() {
    
  }

}
