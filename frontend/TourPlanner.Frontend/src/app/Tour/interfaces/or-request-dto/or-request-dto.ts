import { TransportType } from "../tour-interface/transport-type";
import { Coordinates } from "../tour-interface/coordinates";

export interface ORServiceRequestDto {
    start : Coordinates,
    dest : Coordinates,
    profile : TransportType
}
