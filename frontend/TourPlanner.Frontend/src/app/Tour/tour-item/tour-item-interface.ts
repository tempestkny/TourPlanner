export interface TourItemInterface {
    id : string;
    title: string;
    description?: string;
    from: string;
    to: string;
    transportType: string;

    distance?: string;
    time?: string;
}
