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
import { TourItemService } from '../tour-item/tour-item-service';

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

  editableTour: TourItemInterface = {
    id: '',
    userId: '',
    title: '',
    from: '',
    to: '',
  };

  isFromValid = true;
  isToValid = true;

  constructor(
    private tourListService: TourListService,
    private tourItemService: TourItemService
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
    if (!this.isFromValid || !this.isToValid) {
      return;
    }
    this.saved.emit(this.editableTour);
  }

  checkIfRealPlace(place: string): boolean {
    return true;
  }
}