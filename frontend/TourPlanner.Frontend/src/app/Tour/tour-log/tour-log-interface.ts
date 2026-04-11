// a tour-log consists of date/time, comment, difficulty, total distance, total time, and rating taken
// on the tour
export interface TourLogInterface {
    id : string;
    timeStamp : Date; // Date and Time
    comment? : string;
    difficulty? : Difficulty;
    totalDistance : number; // in Meters
    totalTime : number; //  in Minutes
    rating? : 1 | 2 | 3 | 4 | 5;
}

export enum Difficulty {
  Easy = 'easy',
  Medium = 'medium',
  Hard = 'hard'
}

