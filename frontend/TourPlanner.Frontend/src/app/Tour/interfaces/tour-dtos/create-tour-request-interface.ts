import { TransportType } from "../tour-interface/transport-type";
import { RouteInformation } from "./route-information";

export interface CreateTourRequestInterface {
    title: string,
    description?: string,
    from: string,
    to: string,
    transportType?: TransportType,
    route?: RouteInformation
}