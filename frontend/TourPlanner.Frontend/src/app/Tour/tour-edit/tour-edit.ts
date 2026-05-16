import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';
import { TourItemInterface } from '../tour-item/tour-item-interface';
import { TourListService } from '../tour-list/tour-list-service';

@Component({
  selector: 'app-tour-edit',
  standalone: true,
  imports: [],
  templateUrl: './tour-edit.html',
  styleUrl: './tour-edit.css',
})
export class TourEdit implements OnChanges {
  @Input() tour!: TourItemInterface | null;

  @Output() saved = new EventEmitter<TourItemInterface>();
  @Output() cancel = new EventEmitter<void>();

  validationMessage = '';

  editableTour: TourItemInterface = {
    id: '',
    userId: '',
    title: '',
    from: '',
    to: '',
    transportType: ''
  };

  isFromValid = true;
  isToValid = true;

  constructor(
    private tourListService: TourListService,
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['tour'] && this.tour) {
      this.editableTour = { ...this.tour };
      this.isFromValid = true;
      this.isToValid = true;
    }
  }

  setTitle(value: string): void {
    this.editableTour.title = value;
  }

  setDescription(value: string): void {
    this.editableTour.tourDescription = value;
  }

  setFrom(value: string): void {
    this.isFromValid = this.checkIfRealPlace(value);
    this.editableTour.from = value;
  }

  setTo(value: string): void {
    this.isToValid = this.checkIfRealPlace(value);
    this.editableTour.to = value;
  }

  setTransportType(value: string): void {
    this.editableTour.transportType = value;
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

      if (!this.editableTour.transportType.trim()) {
        this.validationMessage = 'Please select a transport type.';
        return;
      }

    this.saved.emit(this.editableTour);
  }

  checkIfRealPlace(place: string): boolean {
    return true;
  }
}