import { RouteInformation } from "../tour-dtos/route-information";
import { TransportType } from "./transport-type";

export interface TourItemInterface {
    id : string;
    title: string;
    description?: string;
    from: string;
    to: string;
    transportType: TransportType | null;

    route: RouteInformation | null;
}
