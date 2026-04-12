import { Component, Input } from '@angular/core';
import { TourItemInterface } from '../../tour-item/tour-item-interface';
import { TourLogEntry } from "./tour-log-entry/tour-log-entry";
import { Difficulty, TourLogInterface } from '../tour-log-interface';
import { TourLogService } from '../tour-log-service';

@Component({
  selector: 'app-tour-log-list',
  imports: [TourLogEntry],
  templateUrl: './tour-log-list.html',
  styleUrl: './tour-log-list.css',
})
export class TourLogList {
  @Input() tour!: TourItemInterface | null;

  logs: TourLogInterface[] = [
    {
      id: '000',
      timeStamp: new Date(),
      comment: "Nice Tour :)",
      difficulty: Difficulty.Easy,
      totalDistance: 0.01,
      totalTime: 100,
      rating: 1
    },
        {
      id: '001',
      timeStamp: new Date(),
      comment: "Not Nice Tour :>",
      difficulty: Difficulty.Hard,
      totalDistance: 10,
      totalTime: 1,
      rating: 5
    }



  ];
}
