// a tour-log consists of date/time, comment, difficulty, total distance, total time, and rating taken
// on the tour
export interface TourLogInterface {
    id : string;
    tourId : string;
    timeStamp : Date; // Date and Time
    comment? : string;
    difficulty? : Difficulty;
    totalDistance? : number; // in Meters
    totalTime? : number; //  in Minutes
    rating? : number
}

export enum Difficulty {
  Easy = 'easy',
  Medium = 'medium',
  Hard = 'hard'
}

