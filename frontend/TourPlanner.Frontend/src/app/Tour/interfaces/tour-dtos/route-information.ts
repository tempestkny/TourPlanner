import { Coordinates } from "../tour-interface/coordinates"

export interface RouteInformation {
    timeMin : number,
    distKm : number
    route : Coordinates[]
}
