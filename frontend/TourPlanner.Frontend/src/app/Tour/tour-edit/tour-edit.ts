import { Component, Input } from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';


@Component({
  selector: 'app-tour-edit',
  imports: [],
  templateUrl: './tour-edit.html',
  styleUrl: './tour-edit.css',
})
export class TourEdit {
  @Input() tour!: TourItemInterface | null;
}
